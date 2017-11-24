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

namespace Moduware.Tile.Speaker.iOS
{
    public partial class RootViewController : TileViewController
    {
        private bool _active = false;
        private SpeakerTile _speaker;

        private UIImageView speakerButtonOffImage;
        private UIImageView speakerButtonOnImage;

        public RootViewController() : base("RootViewController", null) { }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

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
            ConfigurationApplied += CoreConfigurationApplied;

            speakerButtonOffImage = new UIImageView(View.Frame);
            speakerButtonOffImage.Image = UIImage.FromBundle("SpeakerButtonOff");

            speakerButtonOnImage = new UIImageView(View.Frame);
            speakerButtonOnImage.Image = UIImage.FromBundle("SpeakerButtonOn");


            base.ViewDidLoad(); 
        }

        private void CoreReadyHandler(Object source, EventArgs e)
        {
            _speaker = new SpeakerTile(Core, GetUuidOfTargetModuleOrFirstOfType, new SpeakerNativeTileMethods
            {
                SetSpeakerButtonStateMethod = (active) =>
                {
                    _active = true;
                    SetSpeakerButtonState(true);
                }
            });
        }

        private void SetSpeakerButtonState(bool active)
        {
            if(active)
            {
                RunOnUiThread(() => SpeakerButton.SetBackgroundImage(speakerButtonOnImage.Image, UIControlState.Normal));
            } else
            {
                RunOnUiThread(() => SpeakerButton.SetBackgroundImage(speakerButtonOffImage.Image, UIControlState.Normal));
            }
        }

        partial void SpeakerButtonDown(UIKit.UIButton sender)
        {
            _active = !_active;
            SetSpeakerButtonState(_active);
            if(_active)
            {
                _speaker.TurnOn();
            } else
            {
                _speaker.TurnOff();
            }
        }

        partial void BackButtonDown(UIKit.UIButton sender)
        {
            Utilities.OpenDashboard();
        }

        private void CoreConfigurationApplied(object sender, EventArgs e)
        {
            _speaker.RequestStatus();
        }
    }
}