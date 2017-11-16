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
    [Activity(Label = "Speaker", MainLauncher = true, Theme = "@style/speakerTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new [] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.speaker", Categories = new [] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : TileActivity
    {
        private List<string> targetModuleTypes = new List<string>
        {
            "moduware.module.speaker"
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //ActionBar.SetDisplayHomeAsUpEnabled(true);
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

            var switchOne = FindViewById<Switch>(Resource.Id.defaultSwitch);
            switchOne.CheckedChange += (o, e) =>
            {
                if (e.IsChecked)
                {
                    switchOne.TrackDrawable = GetDrawable(Resource.Mipmap.track_active);
                    switchOne.ThumbDrawable = GetDrawable(Resource.Mipmap.thumb_active);
                }
                else
                {
                    switchOne.TrackDrawable = GetDrawable(Resource.Mipmap.track);
                    switchOne.ThumbDrawable = GetDrawable(Resource.Mipmap.knob);
                }
            };

            // Binding handlers to UI elements
            //var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            //ConfigButton.Click += ConfigButtonClickHandler;

            //var DashboardButton = FindViewById<Button>(Resource.Id.button2);
            //DashboardButton.Click += (s, e) => Utilities.OpenDashboard();

            // We need to know when core is ready so we can start listening for data from gateways
            CoreReady += CoreReadyHandler;
        }

        private void CoreReadyHandler(Object source, EventArgs e)
        {
            /**
            * We can setup lister for received data here
            * you can remove it if your tile not receiving any data from module
            */
            Core.API.Module.DataReceived += ModuleDataReceivedHandler;

            /**
             * You can use raw data event to process raw data from module in byte format without 
             * processing it through module driver
             */
            // Core.API.Module.RawDataReceived += ...;
        }

        private void ModuleDataReceivedHandler(object sender, DriverParseResultEventArgs e)
        {
            var targetModuleUuid = GetUuidOfTargetModuleOrFirstOfType(targetModuleTypes);
            // If there are no supported modules plugged in
            if (targetModuleUuid == Uuid.Empty) return;
            // Ignoring data coming from non-target modules
            if (e.ModuleUUID != targetModuleUuid) return;

            // TODO: here we need to work with parsed data from module somehow 
            
            /**
             * It is a good practice to scope your data to some contexts
             * and first check context before processing data from module
             */
            // if(e.DataSource == "SensorValue") { ... }
           
            // outputing data variables to log
            foreach(var variable in e.Variables)
            {
                Log.Information(variable.Key + "= " + variable.Value);
            }
        }

        private void ConfigButtonClickHandler(Object source, EventArgs e)
        {
            
            //var RedEditbox = FindViewById<EditText>(Resource.Id.editText1);
            //var GreenEditbox = FindViewById<EditText>(Resource.Id.editText2);
            //var BlueEditbox = FindViewById<EditText>(Resource.Id.editText3);

            //// getting color
            //var RedNumber = int.Parse(RedEditbox.Text);
            //var GreenNumber = int.Parse(GreenEditbox.Text);
            //var BlueNumber = int.Parse(BlueEditbox.Text);

            //// We are working with target module or first of type, what is fine for single module use
            //var targetModuleUuid = GetUuidOfTargetModuleOrFirstOfType(targetModuleTypes);

            //// Running command on found module
            //if (targetModuleUuid != Uuid.Empty)
            //{
            //    Core.API.Module.SendCommand(targetModuleUuid, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
            //}
        }
    }
}

