using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Services.Interfaces;
using AirportApp.WinUI;

using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel
{
    public partial class SelectCompanyViewModel : INotifyPropertyChanged
    {
        private readonly ICompanyService companyService;
        private readonly INavigationService navigationService;

        private ObservableCollection<Company> companies = new();
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Company> Companies
        {
            get => companies;
            set
            {
                if (companies != value)
                {
                    companies = value;
                    OnPropertyChanged();
                }
            }
        }

        public SelectCompanyViewModel(ICompanyService companyService, INavigationService navigationService)
        {
            this.companyService = companyService;
            this.navigationService = navigationService;

            _ = LoadCompaniesAsync();
        }

        private async Task LoadCompaniesAsync()
        {
            var availableCompanies = await companyService.GetAllCompaniesAsync();
            Companies = new ObservableCollection<Company>(availableCompanies);
        }

        public IRelayCommand SelectCompanyCommand => new RelayCommand<Company>(SelectCompany);

        private void SelectCompany(Company? company)
        {
            if (company != null)
            {
                navigationService.NavigateTo(typeof(CompanyPage), company.Id);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
