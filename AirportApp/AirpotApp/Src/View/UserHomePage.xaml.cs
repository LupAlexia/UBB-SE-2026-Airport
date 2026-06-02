using AirportApp.Services.Interfaces;
using AirportApp.Src.View.Ticket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.General
{
    public sealed partial class UserHomePage : Page
    {
        private readonly INavigationService navigationService;

        public UserHomePage()
        {
            InitializeComponent();
            navigationService = App.Services.GetRequiredService<INavigationService>();
        }

        private void CustomerSupportButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            navigationService.NavigateTo(typeof(LandingPage));
        }

        private void ManageTicketsButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            navigationService.NavigateTo(typeof(ComplaintTicketPage));
        }

        private void SwitchToEmployeeButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            // Intoarce userul la ChoosingPage ca sa se poata loga ca employee
            navigationService.NavigateTo(typeof(ChoosingPage));
        }
    }
}
