using AirportApp.Src.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI
{
    public sealed partial class HomePage : Page
    {
        public HomeViewModel ViewModel { get; }

        public HomePage()
        {
            ViewModel = App.Services.GetRequiredService<HomeViewModel>();
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
