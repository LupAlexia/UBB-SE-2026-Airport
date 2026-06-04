using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AirportApp.Src.ViewModel
{
    public enum AirportAdminSection
    {
        Flights,
        Employees,
        AirportConfiguration,
        FAQ,
        Tickets,
        Reviews
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
        public ICommand ShowFAQCommand { get; }
        public ICommand ShowTicketsCommand { get; }
        public ICommand ShowReviewsCommand { get; }

        public AirportAdminViewModel()
        {
            ShowFlightsCommand = new RelayCommand(ShowFlights);
            ShowEmployeesCommand = new RelayCommand(ShowEmployees);
            ShowAirportCommand = new RelayCommand(ShowAirport);
            ShowFAQCommand = new RelayCommand(ShowFAQ);
            ShowTicketsCommand = new RelayCommand(ShowTickets);
            ShowReviewsCommand = new RelayCommand(ShowReviews);
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

        private void ShowFAQ()
        {
            SelectedSection = AirportAdminSection.FAQ;
        }

        private void ShowTickets()
        {
            SelectedSection = AirportAdminSection.Tickets;
        }

        private void ShowReviews()
        {
            SelectedSection = AirportAdminSection.Reviews;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
