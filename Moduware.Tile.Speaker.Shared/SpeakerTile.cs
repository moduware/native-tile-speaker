using Moduware.Platform.Core;
using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Core.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moduware.Tile.Speaker.Shared
{
    public class SpeakerTile
    {
        public static List<string> _targetModuleTypes = new List<string>
        {
            "nexpaq.module.speaker", // old classic USB speaker
            "moduware.module.speaker" // modern bluetooth speaker
        };
        public static string Id = "moduware.tile.speaker";
        private Core _core;
        private Func<List<string>, Uuid> _moduleSearchFunc;
        private ISpeakerTileNativeMethods _nativeSpeakerTileMethods;

        public SpeakerTile(Core core, ISpeakerTileNativeMethods nativeSpeakerTileMethods)
        {
            _core = core;
            _moduleSearchFunc = nativeSpeakerTileMethods.GetUuidOfTargetModuleOrFirstOfType;
            _nativeSpeakerTileMethods = nativeSpeakerTileMethods;

            _core.API.Module.DataReceived += ModuleDataReceivedHandler;
            _core.API.Module.TypeRecognised += (o, e) => RequestStatus();
        }

        public void TurnOn()
        {
            var targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            if (targetModuleUuid == Uuid.Empty) return;
            _core.API.Module.SendCommand(targetModuleUuid, "Connect", new int[] { });
        }

        public void TurnOff()
        {
            var targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            if (targetModuleUuid == Uuid.Empty) return;
            _core.API.Module.SendCommand(targetModuleUuid, "Disconnect", new int[] { });
        }

        public void RequestStatus()
        {
            var targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            if (targetModuleUuid == Uuid.Empty) return;
            _core.API.Module.SendCommand(targetModuleUuid, "StatusCheck", new int[] { });
        }

        private void ModuleDataReceivedHandler(object sender, DriverParseResultEventArgs e)
        {
            var targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            // If there are no supported modules plugged in
            if (targetModuleUuid == Uuid.Empty) return;
            // Ignoring data coming from non-target modules
            if (!e.ModuleUUID.Equals(targetModuleUuid)) return;

            if (e.DataSource == "StateChangeResponse" && e.Variables["result"] == "success")
            {
                RequestStatus();
            }
            else if (e.DataSource == "StatusRequestResponse" && e.Variables["status"] == "connected")
            {
                _nativeSpeakerTileMethods.SetSpeakerButtonState(active: true);
            }
        }
    }
}
