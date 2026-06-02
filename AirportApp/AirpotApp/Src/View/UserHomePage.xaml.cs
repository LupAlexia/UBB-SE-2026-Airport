using AirportApp.Services.Interfaces;
using AirportApp.Src.View;
using AirportApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.General
{
    public sealed partial class UserHomePage : Page
    {
        private readonly INavigationService navigationService;
        private readonly ShellViewModel shellViewModel;

        public UserHomePage()
        {
            InitializeComponent();
            navigationService = App.Services.GetRequiredService<INavigationService>();
            shellViewModel = App.Services.GetRequiredService<ShellViewModel>();
        }

        private void CustomerSupportButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            navigationService.NavigateTo(typeof(LandingPage));
        }

        private void ManageTicketsButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            navigationService.NavigateTo(typeof(FlightSearchPage));
        }

        private void DutyFreeButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            shellViewModel.NavigateToShopCommand.Execute(null);
        }

        private void SwitchToEmployeeButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            navigationService.NavigateTo(typeof(ChoosingPage));
        }
    }
}
