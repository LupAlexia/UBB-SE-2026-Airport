using AirportApp.Services.Interfaces;
using AirportApp.WinUI.DutyFreeShops;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI.Pages
{
    public sealed partial class DutyFreeRootPage : Page
    {
        private readonly INavigationService navigationService;

        public DutyFreeRootPage()
        {
            InitializeComponent();
            navigationService = App.Services.GetRequiredService<INavigationService>();
        }

        private void OpenDutyFreeExperience_Click(object sender, RoutedEventArgs e)
        {
            navigationService.NavigateTo(typeof(LandingPage));
        }
    }
}
