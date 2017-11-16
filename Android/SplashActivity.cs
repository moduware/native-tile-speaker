using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Moduware.Tile.Speaker.Droid
{
    [Activity(Label = "Speaker", Theme = "@style/SplashTheme", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            OverridePendingTransition(0, 0);
            Finish();
        }
    }
}