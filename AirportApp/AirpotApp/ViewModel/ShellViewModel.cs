using AirportApp.Services.Interfaces;
using AirportApp.Src.View.General;
using AirportApp.WinUI.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AirportApp.ViewModel
{
    public partial class ShellViewModel : ObservableObject
    {
        private readonly INavigationService navigationService;

        [ObservableProperty]
        private bool isAirportTabActive = true;

        public ShellViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        [RelayCommand]
        private void NavigateToAirport()
        {
            navigationService.NavigateTo(typeof(AirportRootPage));
            IsAirportTabActive = true;
        }

        [RelayCommand]
        private void NavigateToShop()
        {
            navigationService.NavigateTo(typeof(DutyFreeRootPage));
            IsAirportTabActive = false;
        }
    }
}
