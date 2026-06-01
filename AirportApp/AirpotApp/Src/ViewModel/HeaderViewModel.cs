using System.ComponentModel;
using System.Runtime.CompilerServices;

using AirportApp.Services.Interfaces;
using AirportApp.WinUI;

using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel
{
    public partial class HeaderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly INavigationService navigationService;

        public HeaderViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        [RelayCommand]
        private void NavigateHome()
        {
            navigationService.NavigateTo(typeof(HomePage));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
