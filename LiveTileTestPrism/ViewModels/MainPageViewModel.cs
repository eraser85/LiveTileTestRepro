using LiveTileTestPrism.Interfaces;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace LiveTileTestPrism.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public string TestLabel => "View model bound successfully";

        public DelegateCommand PinTileCommand;

        private ITileUpdaterService _tileUpdaterService;
        private ILoggerService _loggerService;
        public MainPageViewModel(ITileUpdaterService tileUpdaterService, ILoggerService loggerService)
        {
            _tileUpdaterService = tileUpdaterService;
            _loggerService = loggerService;

            PinTileCommand = new DelegateCommand(async () => await PinTileAction());
        }

        private async Task PinTileAction()
        {
            var guid = Guid.NewGuid();
            var tile = new SecondaryTile(guid.ToString())
            {
                DisplayName = "Test tile",
                Arguments = guid.ToString()
            };
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png", UriKind.Absolute);

            if (!await tile.RequestCreateAsync())
                return;

            var tileNotification = new TileNotification(_tileUpdaterService.GenerateTileContent().GetXml());
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(tileNotification);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
        }
    }
}
