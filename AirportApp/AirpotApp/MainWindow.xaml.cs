using AirportApp.Helpers;
using AirportApp.WinUI;
using Microsoft.UI.Xaml;

namespace AirportApp
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowHelper.MaximizeWindow(this);
            RootFrame.Navigate(typeof(ShellPage));
        }
    }
}
