using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTileTestPrism.Interfaces
{
    public interface ITileUpdaterService
    {
        void UnregisterBackgroundTasks();
        Task<bool> RegisterBackgroundTasks();

        Task RefreshAllTiles();
        TileContent GenerateTileContent();
    }
}
