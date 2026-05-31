using AirportApp.Services.Interfaces;
using AirportApp.Src.View.General;
using Microsoft.Extensions.DependencyInjection;
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

        private void OpenDutyFreeExperience_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            navigationService.NavigateTo(typeof(LandingPage));
        }
    }
}
