using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE;
using Serilog;
using System.Threading.Tasks;
using Android.Content;
using System;
using Moduware.Platform.Tile.Droid;
using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Core.EventArguments;
using System.Collections.Generic;
using Moduware.Tile.Speaker.Shared;
using Plugin.BLE.Abstractions.Contracts;
using Android.Bluetooth;

namespace Moduware.Tile.Speaker.Droid
{
    [Activity(Label = "Speaker", Theme = "@style/speakerTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new[] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.speaker", Categories = new[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : TileActivity, ISpeakerTileNativeMethods
    {
        private ImageButton _speakerButton;
        private Switch _defaultSwitch;
        private bool _active = false;
        private SpeakerTile _speaker;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // We need assign Id of our tile here, it is required for proper Dashboard - Tile communication
            TileId = SpeakerTile.Id;

            // Logger to output messages from PlatformCore to console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .CreateLogger();

            Window.RequestFeature(Android.Views.WindowFeatures.CustomTitle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Window.SetFeatureInt(Android.Views.WindowFeatures.CustomTitle, Resource.Layout.Header);
            //Window.SetStatusBarColor(new Android.Graphics.Color(180, 64, 60));

            SetupUiListeners();

            // We need to know when core is ready so we can start listening for data from gateways
            CoreReady += CoreReadyHandler;
            // And we need to know when we are ready to send commands
            ConfigurationApplied += CoreConfigurationAppliedHandler;
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
            _speaker.SetupTargetModule();
            _speaker.RequestStatus();
        }

        #endregion

#region UiEventListeners

        /// <summary>
        /// We need listen for clicks on back button, main speaker button and default state switch
        /// </summary>
        private void SetupUiListeners()
        {
            var backButton = FindViewById<ImageButton>(Resource.Id.back_button);
            backButton.Click += (o, e) => Utilities.OpenDashboard();

            _speakerButton = FindViewById<ImageButton>(Resource.Id.speaker_button);
            _speakerButton.Click += SpeakerButtonClickHandler;

            _defaultSwitch = FindViewById<Switch>(Resource.Id.defaultSwitch);
            _defaultSwitch.CheckedChange += DefaultSwitchStateCheckedChange;
        }

        /// <summary>
        /// When default state switch change commanding speaker to change config and switching image accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefaultSwitchStateCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            _speaker.ChangeSpeakerDefaultState(e.IsChecked);
            RunOnUiThread(() =>
            {
                if (e.IsChecked)
                {
                    _defaultSwitch.TrackDrawable = GetDrawable(Resource.Mipmap.track_active);
                    _defaultSwitch.ThumbDrawable = GetDrawable(Resource.Mipmap.thumb_active);
                }
                else
                {
                    _defaultSwitch.TrackDrawable = GetDrawable(Resource.Mipmap.track);
                    _defaultSwitch.ThumbDrawable = GetDrawable(Resource.Mipmap.knob);
                }
            });
            
        }

        /// <summary>
        /// When main speaker button clicked we need send command to speaker module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakerButtonClickHandler(object sender, EventArgs e)
        {
            SetSpeakerButtonState(!_active);

            if (_active)
            {
                _speaker.TurnOn();
            } else
            {
                _speaker.TurnOff();
            }
        }

        #endregion

#region UiMethods

        /// <summary>
        /// Change state of main speaker button in UI
        /// </summary>
        /// <param name="active"></param>
        public void SetSpeakerButtonState(bool active)
        {
            _active = active;
            if (active)
            {
                RunOnUiThread(() => _speakerButton.SetImageDrawable(GetDrawable(Resource.Drawable.speaker_button_on)));
            }
            else
            {
                RunOnUiThread(() => _speakerButton.SetImageDrawable(GetDrawable(Resource.Drawable.speaker_button_off)));
            }
        }
        
        /// <summary>
        /// Change state of default speaker state switch
        /// </summary>
        /// <param name="active"></param>
        public void SetSpeakerDefaultState(bool active)
        {
            RunOnUiThread(() => _defaultSwitch.Checked = active);
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
                    ((BluetoothDevice)device.NativeDevice).CreateBond();

                    return true;
                }
                return false;
            });
        }
    }
}

