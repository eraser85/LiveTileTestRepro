using Windows.UI.Xaml.Controls;

namespace LiveTileTestPrism
{
    public sealed partial class AppShell : Page
    {
        public AppShell()
        {
            InitializeComponent();
        }

        public void SetContentFrame(Frame frame)
        {
            contentFrame.Content = frame;
        }
    }
}
