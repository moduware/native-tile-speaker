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
using Moduware.Platform.Tile.Shared;

namespace Moduware.Platform.Tile.Droid
{
    class ProgressScreenLock : IProgressScreenLock
    {
        private Activity _context;
        private int _dialogsTheme = 5;
        private AlertDialog _dialog;

        public ProgressScreenLock(Activity context)
        {
            _context = context;
        }

        public void Setup(string title, string message)
        {
            _context.RunOnUiThread(() =>
            {
                var DialogBuilder = new AlertDialog.Builder(_context, _dialogsTheme);
                DialogBuilder.SetTitle(title);
                DialogBuilder.SetMessage(message);
                DialogBuilder.SetCancelable(false);

                _dialog = DialogBuilder.Create();
            });
        }

        public void Show()
        {
            if (_dialog == null) throw new MissingMemberException("You need call Setup before showing screen lock");
            _context.RunOnUiThread(() => _dialog.Show());
        }

        public void Hide()
        {
            if (_dialog == null) throw new MissingMemberException("You need call Setup before hiding screen lock");
            _context.RunOnUiThread(() => _dialog.Hide());
        }
    }
}