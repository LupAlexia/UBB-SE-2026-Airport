using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AirportApp.Src.ViewModel
{
    public enum AirportAdminSection
    {
        Flights,
        Employees,
        AirportConfiguration
    }

    public partial class AirportAdminViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private AirportAdminSection selectedSection = AirportAdminSection.Flights;

        public AirportAdminSection SelectedSection
        {
            get => selectedSection;
            set
            {
                if (selectedSection != value)
                {
                    selectedSection = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ShowFlightsCommand { get; }
        public ICommand ShowEmployeesCommand { get; }
        public ICommand ShowAirportCommand { get; }

        public AirportAdminViewModel()
        {
            ShowFlightsCommand = new RelayCommand(ShowFlights);
            ShowEmployeesCommand = new RelayCommand(ShowEmployees);
            ShowAirportCommand = new RelayCommand(ShowAirport);
        }

        public void Initialize()
        {
            SelectedSection = AirportAdminSection.Flights;
        }
        private void ShowFlights()
        {
            SelectedSection = AirportAdminSection.Flights;
        }
        private void ShowEmployees()
        {
            SelectedSection = AirportAdminSection.Employees;
        }

        private void ShowAirport()
        {
            SelectedSection = AirportAdminSection.AirportConfiguration;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
