using Moduware.Platform.Core;
using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Core.EventArguments;
using System;
using System.Collections.Generic;

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
        private string _targetModuleType;

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
            _core.API.Module.Pulled += ModuleDisconnectedHandler;
            _core.API.Gateway.Disconnected += ModuleDisconnectedHandler;

            _core.API.Module.DataReceived += ModuleDataReceivedHandler;
            _core.API.Module.TypeRecognised += (o, e) => RequestStatus();   
        }

        /// <summary>
        /// If there are any supported module plugged in with preference to target module using it,
        /// if there are no supported modules, showing alert and openning dashboard
        /// </summary>
        public void SetupTargetModule()
        {
            bool noModule = false;
            _targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            if (_targetModuleUuid != Uuid.Empty)
            {
                var module = _core.API.Module.GetByUUID(_targetModuleUuid);
                if (module == null)
                {
                    noModule = true;
                } else
                {
                    _targetModuleType = module.TypeID;
                }

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

#region ModuleMethods

        /// <summary>
        /// Turn speaker on
        /// </summary>
        public void TurnOn()
        {
            _core.API.Module.SendCommand(_targetModuleUuid, "Connect", new int[] { });
            if(_targetModuleType == "moduware.module.speaker" && _bluetoothName == String.Empty)
            {
                AskBluetoothName();
            }
        }

        /// <summary>
        /// Turn speaker off
        /// </summary>
        public void TurnOff()
        {
            _core.API.Module.SendCommand(_targetModuleUuid, "Disconnect", new int[] { });
        }

        /// <summary>
        /// Request current status of speaker
        /// </summary>
        public void RequestStatus()
        {
            _core.API.Module.SendCommand(_targetModuleUuid, "StatusCheck", new int[] { });
        }

        /// <summary>
        /// Ask for bluetooth speaker name, for moduware.module.speaker only
        /// </summary>
        public void AskBluetoothName()
        {
            _core.API.Module.SendCommand(_targetModuleUuid, "AskBluetoothName", new int[] { });
        }

        /// <summary>
        /// Configure speaker for another default state
        /// </summary>
        /// <param name="active">Default state for speaker</param>
        public void ChangeSpeakerDefaultState(bool active)
        {
            if(active)
            {
                _core.API.Module.SendCommand(_targetModuleUuid, "SetDefaultStateAsOn", new int[] { });
            } else
            {
                _core.API.Module.SendCommand(_targetModuleUuid, "SetDefaultStateAsOff", new int[] { });
            }
        }

        #endregion

#region ModuleEventHandlers

        /// <summary>
        /// After module disconnected we need make sure we can continue working
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ModuleDisconnectedHandler(Object o, EventArgs e)
        {
            SetupTargetModule();
            _bluetoothName = String.Empty;
        }

        /// <summary>
        /// Handle data coming from module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Module message parsed using driver</param>
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
            else if (e.DataSource == "StatusRequestResponse") {
                if (e.Variables["status"] == "connected")
                {
                    _nativeMethods.SetSpeakerButtonState(active: true);
                }
                if(_targetModuleType == "moduware.module.speaker")
                {
                    var defaultState = e.Variables["defaultState"] == "connected";
                    _nativeMethods.SetSpeakerDefaultState(defaultState);
                }
            } else if(e.DataSource == "BluetoothNameRequestResponse")
            {
                _bluetoothName = e.Variables["bluetoothName"];
                // Pairing device
                _nativeMethods.PairToBluetoothDevice(_bluetoothName);
            }
        }

#endregion

    }
}
