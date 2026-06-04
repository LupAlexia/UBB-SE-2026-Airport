using AirportApp.Services.Interfaces;
using AirportApp.WinUI;
using AirportApp.WinUI.AirportAdmin;
using AirportApp.WinUI.Pages;
using AirportApp.WinUI.StaffLogin;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AirportApp.Src.ViewModel
{
    public partial class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly INavigationService navigationService;

        public HomeViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        [RelayCommand]
        private void NavigateToCompany()
        {
            navigationService.NavigateTo(typeof(SelectCompanyPage));
        }

        [RelayCommand]
        private void NavigateToAdmin()
        {
            navigationService.NavigateTo(typeof(AirportAdminPage));
        }

        [RelayCommand]
        private void NavigateToStaff()
        {
            navigationService.NavigateTo(typeof(StaffLoginPage));
        }

        [RelayCommand]
        private void NavigateToAirport()
        {
            navigationService.NavigateTo(typeof(AirportRootPage));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
