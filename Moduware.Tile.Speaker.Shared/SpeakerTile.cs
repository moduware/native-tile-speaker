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
        private Uuid _targetModuleUuid;

        private Func<List<string>, Uuid> _moduleSearchFunc;
        private ISpeakerTileNativeMethods _nativeMethods;
        private string _bluetoothName = String.Empty;

        public SpeakerTile(Core core, ISpeakerTileNativeMethods nativeMethods)
        {
            _core = core;
            _moduleSearchFunc = nativeMethods.GetUuidOfTargetModuleOrFirstOfType;
            _nativeMethods = nativeMethods;

            // After configuration recieved we need find module we want work with
            _nativeMethods.ConfigurationApplied += (o, e) => SetupTargetModule();
            // If module was pulled, we need to check if there are still supported module 
            _core.API.Module.Pulled += (o, e) => SetupTargetModule();

            _core.API.Module.DataReceived += ModuleDataReceivedHandler;
            _core.API.Module.TypeRecognised += (o, e) => RequestStatus();
            
        }

        public void SetupTargetModule()
        {
            bool noModule = false;
            _targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            if (_targetModuleUuid != Uuid.Empty)
            {
                var module = _core.API.Module.GetByUUID(_targetModuleUuid);
                if (module == null) noModule = true;

            } else
            {
                noModule = true;
            }
            
            if(noModule) {
                _nativeMethods.Utilities.ShowNoSupportedModuleAlert(() =>
                {
                    _nativeMethods.Utilities.OpenDashboard();
                });
            }
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

        // moduware.module.speaker only
        public void AskBluetoothName()
        {
            var targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            if (targetModuleUuid == Uuid.Empty) return;
            var module = _core.API.Module.GetByUUID(targetModuleUuid);
            if (module.TypeID != "moduware.module.speaker") return;
            _core.API.Module.SendCommand(targetModuleUuid, "AskBluetoothName", new int[] { });
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
                _nativeMethods.SetSpeakerButtonState(active: true);
            } else if(e.DataSource == "BluetoothNameRequestResponse")
            {
                _bluetoothName = e.Variables["bluetoothName"];
            }
        }
    }
}
