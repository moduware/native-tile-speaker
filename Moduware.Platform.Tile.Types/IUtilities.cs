using System;
using System.Collections.Generic;
using System.Text;

namespace Moduware.Platform.Tile.Types
{
    public interface IUtilities
    {
        void ShowNotConnectedAlert(Action callback);
        void ShowNoSupportedModuleAlert(Action callback);
        void OpenDashboard(string request = "");
    }
}
