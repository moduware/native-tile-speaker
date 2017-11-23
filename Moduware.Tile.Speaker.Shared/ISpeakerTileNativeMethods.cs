using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moduware.Tile.Speaker.Shared
{
    public interface ISpeakerTileNativeMethods
    {
        void SetSpeakerButtonState(bool active);
    }
}
