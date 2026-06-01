using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.WinUI.AirportAdmin.Components;

using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

namespace AirportApp.Src.ViewModel
{
    public partial class FlightsDashboardViewModel : INotifyPropertyChanged
    {
        private readonly IFlightRouteService flightRouteService;
        private readonly IEmployeeFlightService flightEmployeeService;

        private List<Flight> allFlights = new();

        private string searchText = string.Empty;
        private FlightDisplayRow? selectedFlight;
        private Visibility crewDialogVisibility = Visibility.Collapsed;
        private string dialogError = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public FlightsDashboardViewModel(
           IFlightRouteService flightRouteService,
           IEmployeeFlightService flightEmployeeService)
        {
            this.flightRouteService = flightRouteService;
            this.flightEmployeeService = flightEmployeeService;
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        public FlightDisplayRow? SelectedFlight
        {
            get => selectedFlight;
            set
            {
                if (selectedFlight != value)
                {
                    selectedFlight = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility CrewDialogVisibility
        {
            get => crewDialogVisibility;
            set
            {
                if (crewDialogVisibility != value)
                {
                    crewDialogVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DialogError
        {
            get => dialogError;
            set
            {
                if (dialogError != value)
                {
                    dialogError = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<CrewSelectionWrapper> AvailableCrew { get; } = new();
        public ObservableCollection<FlightDisplayRow> FilteredFlights { get; } = new();

        [RelayCommand]
        public void LoadFlights()
        {
            allFlights = RunSync(() => flightRouteService.GetAllFlightsWithDetailsAsync()).ToList();
            ApplyFilter();
        }

        [RelayCommand]
        private void OpenCrewManagement()
        {
            if (SelectedFlight == null)
            {
                return;
            }

            Flight? flight = RunSync(() => flightRouteService.GetFlightByIdAsync(SelectedFlight.Id));
            if (flight == null)
            {
                return;
            }

            var crewData = RunSync(() => flightEmployeeService.GetCrewSelectionDataAsync(flight)).ToList();

            AvailableCrew.Clear();
            foreach (CrewMemberSelectionData item in crewData)
            {
                AvailableCrew.Add(new CrewSelectionWrapper(item.Employee)
                {
                    IsSelected = item.IsSelected,
                    RoleHeader = item.RoleHeader,
                    RoleHeaderVisibility = item.IsFirstInRoleGroup ? Visibility.Visible : Visibility.Collapsed
                });
            }

            DialogError = string.Empty;
            CrewDialogVisibility = Visibility.Visible;
        }

        [RelayCommand]
        private void SaveCrew()
        {
            if (SelectedFlight == null)
            {
                return;
            }

            List<int> selectedEmployeeIds = new List<int>();
            foreach (CrewSelectionWrapper selectionContext in AvailableCrew)
            {
                if (selectionContext.IsSelected)
                {
                    selectedEmployeeIds.Add(selectionContext.Employee.Id);
                }
            }

            RunSync(() => flightEmployeeService.UpdateEmployeesForFlightUsingIdsAsync(SelectedFlight.Id, selectedEmployeeIds));
            CrewDialogVisibility = Visibility.Collapsed;
            LoadFlights();
        }

        [RelayCommand]
        private void CloseDialog()
        {
            CrewDialogVisibility = Visibility.Collapsed;
        }

        private void ApplyFilter()
        {
            string query = SearchText?.Trim().ToLowerInvariant() ?? string.Empty;
            var matchingFlights = RunSync(() => flightRouteService.SearchFlightsAsync(allFlights, query)).ToList();

            FilteredFlights.Clear();
            foreach (Flight flight in matchingFlights)
            {
                string crewText = flightEmployeeService.FormatCrewList(flight.Id);
                FilteredFlights.Add(new FlightDisplayRow(RunSync(() => flightRouteService.BuildFlightSummaryAsync(flight, crewText))));
            }
        }

        private static T RunSync<T>(Func<Task<T>> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();

        private static void RunSync(Func<Task> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CrewSelectionWrapper
    {
        public CrewSelectionWrapper(Employee employee)
        {
            this.Employee = employee;
        }
        public Employee Employee { get; set; }
        public bool IsSelected { get; set; }
        public bool ShowRoleHeader { get; set; }
        public string RoleHeader { get; set; } = string.Empty;
        public Visibility RoleHeaderVisibility { get; set; } = Visibility.Collapsed;
    }
}
