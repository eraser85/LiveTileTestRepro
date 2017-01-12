using LiveTileTestPrism.ViewModels;
using Prism.Windows.Mvvm;
using System.ComponentModel;
using Windows.UI.Xaml;


namespace LiveTileTestPrism.Views
{
    public sealed partial class MainPage : SessionStateAwarePage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainPageViewModel ConcreteDataContext => DataContext as MainPageViewModel;

        public MainPage()
        {
            InitializeComponent();
            DataContextChanged += MainPage_DataContextChanged;
        }


        private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConcreteDataContext)));
        }
    }
}
