﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Moduware.Platform.Tile.Shared
{
    public interface IProgressScreenLock
    {
        void Setup(string title, string message);
        void Show();
        void Hide();
    }
}
