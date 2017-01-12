using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LiveTileTestSimple
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += async (o, e) => await Common.RegisterBackgroundTasks();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await PinTile();
        }

        private async Task PinTile()
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

            var tileNotification = new TileNotification(Common.GenerateTileContent().GetXml());
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(tileNotification);
        }
        
    }
}
