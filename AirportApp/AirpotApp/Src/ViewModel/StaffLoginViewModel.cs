using System.ComponentModel;
using System.Runtime.CompilerServices;

using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Services.Interfaces;
using AirportApp.WinUI;

using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

namespace AirportApp.Src.ViewModel
{
    public partial class StaffLoginViewModel : INotifyPropertyChanged
    {
        private const string ErrorMessageFailedLogin = "Login failed";

        private readonly IEmployeeService employeeService;
        private readonly INavigationService navigationService;

        public event PropertyChangedEventHandler? PropertyChanged;

        private string employeeIdText = string.Empty;
        public string EmployeeIdText
        {
            get => employeeIdText;
            set
            {
                if (employeeIdText != value)
                {
                    employeeIdText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility errorVisibility = Visibility.Collapsed;
        public Visibility ErrorVisibility
        {
            get => errorVisibility;
            set
            {
                if (errorVisibility != value)
                {
                    errorVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public StaffLoginViewModel(IEmployeeService employeeService, INavigationService navigationService)
        {
            this.employeeService = employeeService;
            this.navigationService = navigationService;
        }

        [RelayCommand]
        private async Task Login()
        {
            try
            {
                int employeeId = await employeeService.LoginAsync(EmployeeIdText);

                ErrorVisibility = Visibility.Collapsed;
                ErrorMessage = string.Empty;

                navigationService.NavigateTo(typeof(StaffPage), employeeId);
            }
            catch (Exception exception)
            {
                ShowError(ErrorMessageFailedLogin + ": " + exception.Message);
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            ErrorVisibility = Visibility.Visible;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
