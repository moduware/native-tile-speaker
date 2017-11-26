using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Tile.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moduware.Tile.Speaker.Shared
{
    public interface ISpeakerTileNativeMethods
    {
        IUtilities Utilities { get; }

        void SetSpeakerButtonState(bool active);
        Uuid GetUuidOfTargetModuleOrFirstOfType(List<string> list);
        void PairToBluetoothDevice(string name);

        event EventHandler ConfigurationApplied;
    }
}
