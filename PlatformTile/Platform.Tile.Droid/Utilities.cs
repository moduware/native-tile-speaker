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
using Moduware.Platform.Tile.Types;
using System.Threading.Tasks;

namespace Moduware.Platform.Tile.Droid
{
    public class Utilities : IUtilities
    {
        private Context context;

        public Utilities(Context context)
        {
            this.context = context;
        }

        public async void ShowNoSupportedModuleAlert(Action callback)        {            await ShowAlertAsync("No module", "No modules supported by this tile plugged in, please insert one to use tile.", "OK");            callback();
        }

        public async void ShowNotConnectedAlert(Action callback)
        {
            await ShowAlertAsync("Not connected", "You are not connected to any moduware device, please search and connect to one.", "OK");            callback();
        }

        public Task ShowAlertAsync(string title, string message, string buttonText)        {
            var t = new TaskCompletionSource<bool>();

            int DialogsTheme = 5;
            var DialogBuilder = new AlertDialog.Builder(context, DialogsTheme);
            DialogBuilder.SetTitle(title);
            DialogBuilder.SetMessage(message);
            DialogBuilder.SetCancelable(false);
            DialogBuilder.SetPositiveButton(buttonText, (sender, e) => {
                t.TrySetResult(true);
            });

            (context as Activity).RunOnUiThread(action: () => {
                var AlertDialog = DialogBuilder.Create();
                AlertDialog.Show();
            });

            return t.Task;
        }

            public void OpenDashboard(string request = "")
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("moduware.application.dashboard://" + request));
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
    }
}