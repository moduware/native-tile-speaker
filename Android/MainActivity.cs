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

namespace Moduware.Tile.Speaker.Droid
{
    [Activity(Label = "Speaker", Theme = "@style/speakerTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new[] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.speaker", Categories = new[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : TileActivity
    {
        private ImageButton _speakerButton;
        private Switch _defaultSwitch;
        private bool _active = false;
        

        private List<string> targetModuleTypes = new List<string>
        {
            "nexpaq.module.speaker", // old classic USB speaker
            "moduware.module.speaker" // modern bluetooth speaker
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // We need assign Id of our tile here, it is required for proper Dashboard - Tile communication
            TileId = "moduware.tile.speaker";

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
            ConfigurationApplied += CoreConfigurationApplied;
        }

        private void SetupUiListeners()
        {
            var backButton = FindViewById<ImageButton>(Resource.Id.back_button);
            backButton.Click += (o, e) => Utilities.OpenDashboard();

            _speakerButton = FindViewById<ImageButton>(Resource.Id.speaker_button);
            _speakerButton.Click += SpeakerButtonClickHandler;

            _defaultSwitch = FindViewById<Switch>(Resource.Id.defaultSwitch);
            _defaultSwitch.CheckedChange += DefaultSwitchStateCheckedChange;
        }

        private void DefaultSwitchStateCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
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
        }

        private void SetSpeakerButtonState(bool state)
        {
            RunOnUiThread(() =>
            {
                if (state)
                {
                    _speakerButton.SetImageDrawable(GetDrawable(Resource.Drawable.speaker_button_on));
                }
                else
                {
                    _speakerButton.SetImageDrawable(GetDrawable(Resource.Drawable.speaker_button_off));
                }
            });
        }

        private void SpeakerButtonClickHandler(object sender, EventArgs e)
        {
            _active = !_active;

            var targetModuleUuid = GetUuidOfTargetModuleOrFirstOfType(targetModuleTypes);
            if (targetModuleUuid == Uuid.Empty) return;
            if (_active)
            {
                SetSpeakerButtonState(true);

                Core.API.Module.SendCommand(targetModuleUuid, "Connect", new int[] { });
            } else
            {
                SetSpeakerButtonState(false);

                Core.API.Module.SendCommand(targetModuleUuid, "Disconnect", new int[] { });
            }
        }

        private void RequestStatus()
        {
            var targetModuleUuid = GetUuidOfTargetModuleOrFirstOfType(targetModuleTypes);
            if (targetModuleUuid == Uuid.Empty) return;
            Core.API.Module.SendCommand(targetModuleUuid, "StatusCheck", new int[] { });
        }

        private void CoreReadyHandler(Object source, EventArgs _e)
        {
            Core.API.Module.DataReceived += ModuleDataReceivedHandler;
            Core.API.Module.TypeRecognised += (o, e) => RequestStatus();
        }

        private void CoreConfigurationApplied(object sender, EventArgs e)
        {
            RequestStatus();
        }

        private void ModuleDataReceivedHandler(object sender, DriverParseResultEventArgs e)
        {
            var targetModuleUuid = GetUuidOfTargetModuleOrFirstOfType(targetModuleTypes);
            // If there are no supported modules plugged in
            if (targetModuleUuid == Uuid.Empty) return;
            // Ignoring data coming from non-target modules
            if (!e.ModuleUUID.Equals(targetModuleUuid)) return;

            if(e.DataSource == "StateChangeResponse" && e.Variables["result"] == "success")
            {
                RequestStatus();
            }
            else if(e.DataSource == "StatusRequestResponse" && e.Variables["status"] == "connected")
            {
                _active = true;
                SetSpeakerButtonState(true);
            }
        }
    }
}

