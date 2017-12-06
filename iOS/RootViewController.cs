using System;

using UIKit;
using Moduware.Platform.Core;
using Plugin.BLE;
using System.Threading.Tasks;
using Serilog;
using Moduware.Platform.Tile.iOS;
using Moduware.Platform.Core.CommonTypes;
using System.Collections.Generic;
using Moduware.Platform.Core.EventArguments;
using Moduware.Tile.Speaker.Shared;
using CoreBluetooth;

namespace Moduware.Tile.Speaker.iOS
{
    public partial class RootViewController : TileViewController, ISpeakerTileNativeMethods
    {
        private bool _active = false;
        private SpeakerTile _speaker;

        private UIImageView speakerButtonOffImage;
        private UIImageView speakerButtonOnImage;

        public RootViewController() : base("RootViewController", null) { }

        public override void ViewDidLoad()
        {
            // We need assign Id of our tile here, it is required for proper Dashboard - Tile communication
            TileId = SpeakerTile.Id;

            // Logger to output messages from PlatformCore to console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.NSLog()
                .CreateLogger();

            // We need to know when core is ready so we can start listening for data from gateways
            CoreReady += CoreReadyHandler;
            // And we need to know when we are ready to send commands
            ConfigurationApplied += CoreConfigurationAppliedHandler;

            LoadButtonImages();
            base.ViewDidLoad(); 
        }

#region CoreEventHandlers

        /// <summary>
        /// When core is ready initialising our shared code
        /// </summary>
        /// <param name="source"></param>
        /// <param name="_e"></param>
        private void CoreReadyHandler(Object source, EventArgs _e)
        {
            _speaker = new SpeakerTile(Core, this);
        }

        /// <summary>
        /// When we recieved configuration from main app, requesting speaker status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoreConfigurationAppliedHandler(object sender, EventArgs e)
        {
            _speaker.RequestStatus();
        }

        #endregion

#region UiEventHandlers

        /// <summary>
        /// When main speaker button clicked we need send command to speaker module
        /// </summary>
        /// <param name="sender"></param>
        partial void SpeakerButtonDown(UIKit.UIButton sender)
        {
            SetSpeakerButtonState(!_active);
            if (_active)
            {
                _speaker.TurnOn();
            }
            else
            {
                _speaker.TurnOff();
            }
        }

        /// <summary>
        /// When back button pressed we need open dashboard
        /// </summary>
        /// <param name="sender"></param>
        partial void BackButtonDown(UIKit.UIButton sender)
        {
            Utilities.OpenDashboard();
        }

        /// <summary>
        /// When default state switch change commanding speaker to change config
        /// </summary>
        /// <param name="sender"></param>
        partial void DefaultStateButtonChange(UISwitch sender)
        {
            _speaker.ChangeSpeakerDefaultState(sender.Enabled);
        }

        #endregion

#region UiMethods

        /// <summary>
        /// Loading button images so we can switch them at runtime
        /// </summary>
        private void LoadButtonImages()
        {
            speakerButtonOffImage = new UIImageView(View.Frame);
            speakerButtonOffImage.Image = UIImage.FromBundle("SpeakerButtonOff");

            speakerButtonOnImage = new UIImageView(View.Frame);
            speakerButtonOnImage.Image = UIImage.FromBundle("SpeakerButtonOn");
        }

        /// <summary>
        /// Change state of main speaker button in UI
        /// </summary>
        /// <param name="active"></param>
        public void SetSpeakerButtonState(bool active)
        {
            _active = active;
            if (active)
            {
                RunOnUiThread(() => SpeakerButton.SetBackgroundImage(speakerButtonOnImage.Image, UIControlState.Normal));
            }
            else
            {
                RunOnUiThread(() => SpeakerButton.SetBackgroundImage(speakerButtonOffImage.Image, UIControlState.Normal));
            }
        }

        /// <summary>
        /// Change state of default speaker state switch
        /// </summary>
        /// <param name="active"></param>
        public void SetSpeakerDefaultState(bool active)
        {
            DefaultSwitch.Enabled = active;
        }

        #endregion

        /// <summary>
        /// For moduware speaker we need establish secondary Bluetooth connection
        /// </summary>
        /// <param name="name">Bluetooth device name</param>
        public void PairToBluetoothDevice(string name)
        {
            var adapter = CrossBluetoothLE.Current.Adapter;
            adapter.StartScanningForDevicesAsync(deviceFilter: (device) =>
            {
                if (device.Name == name)
                {
                    adapter.StopScanningForDevicesAsync();
                    // Pairing device
                    adapter.ConnectToDeviceAsync(device);

                    return true;
                }
                return false;
            });
        }
    }
}