using Moduware.Platform.Tile.Types;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace Moduware.Platform.Tile.iOS
{
    class ProgressScreenLock : IProgressScreenLock
    {
        private Action<Action> _runOnUiThread;
        private UIAlertController _alert;

        public ProgressScreenLock(Action<Action> runOnUiThread)
        {
            _runOnUiThread = runOnUiThread;
        }

        public void Setup(string title, string message)
        {
            _runOnUiThread(() =>
            {
                _alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            });
        }

        public void Show()
        {
            _runOnUiThread(() =>
            {
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(_alert, true, null);
            });
        }

        public void Hide()
        {
            _runOnUiThread(() =>
            {
                _alert.DismissViewController(true, null);
            });
        }
    }
}
