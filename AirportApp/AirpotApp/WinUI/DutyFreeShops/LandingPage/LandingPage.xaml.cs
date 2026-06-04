using AirportApp.Src.ViewModel;
using AirportApp.WinUI.DutyFreeShops.ShopPage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI.DutyFreeShops
{
    public sealed partial class LandingPage : Page
    {
        public IDutyFreeLandingViewModel ViewModel { get; }

        public LandingPage()
        {
            ViewModel = App.Services.GetRequiredService<IDutyFreeLandingViewModel>();
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectClientCommand.Execute(null);
            if (ViewModel.IsRoleSelected)
            {
                Frame.Navigate(typeof(DutyFreeShopPage));
            }
            else
            {
                ErrorText.Text = ViewModel.ErrorMessage;
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectAdminCommand.Execute(null);
            if (ViewModel.IsRoleSelected)
            {
                Frame.Navigate(typeof(DutyFreeShopPage));
            }
            else
            {
                ErrorText.Text = ViewModel.ErrorMessage;
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}
