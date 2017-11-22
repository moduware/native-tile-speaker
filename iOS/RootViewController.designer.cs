// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Moduware.Tile.Speaker.iOS
{
    [Register ("RootViewController")]
    partial class RootViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BackButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch DefaultSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SpeakerButton { get; set; }

        [Action ("SpeakerButtonDown:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SpeakerButtonDown (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BackButton != null) {
                BackButton.Dispose ();
                BackButton = null;
            }

            if (DefaultSwitch != null) {
                DefaultSwitch.Dispose ();
                DefaultSwitch = null;
            }

            if (SpeakerButton != null) {
                SpeakerButton.Dispose ();
                SpeakerButton = null;
            }
        }
    }
}