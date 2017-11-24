using System;

namespace Moduware.Tile.Speaker.Shared
{
    public class SpeakerNativeTileMethods : ISpeakerTileNativeMethods
    {
        public Action<bool> SetSpeakerButtonStateMethod;

        public void SetSpeakerButtonState(bool active)
        {
            if (SetSpeakerButtonStateMethod == null) return;
            SetSpeakerButtonStateMethod(active);
        }
    }
}