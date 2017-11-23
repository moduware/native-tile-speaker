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
    class ProgressScreenLock
    {
        private Activity _context;
        private int _dialogsTheme = 5;
        private AlertDialog _dialog;

        public ProgressScreenLock(Activity context, string title, string message)
        {
            _context = context;
            context.RunOnUiThread(() =>
            {
                var DialogBuilder = new AlertDialog.Builder(context, _dialogsTheme);
                DialogBuilder.SetTitle(title);
                DialogBuilder.SetMessage(message);
                DialogBuilder.SetCancelable(false);

                _dialog = DialogBuilder.Create();
            });
        }

        public void Show()
        {
            _context.RunOnUiThread(() => _dialog.Show());
        }

        public void Hide()
        {
            _context.RunOnUiThread(() => _dialog.Hide());
        }
    }
}