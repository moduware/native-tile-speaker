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
using Moduware.Tile.Speaker.Shared;

namespace Moduware.Tile.Speaker.Droid
{
    class SpeakerNativeTileMethods : ISpeakerTileNativeMethods
    {
        public Action<bool> SetSpeakerButtonStateMethod;

        public void SetSpeakerButtonState(bool active)
        {
            if (SetSpeakerButtonStateMethod == null) return;
            SetSpeakerButtonStateMethod(active);
        }
    }
}