using System.ComponentModel;

using AirportApp.Src.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI.AirportAdmin
{
    public sealed partial class AirportAdminPage : Page
    {
        public AirportAdminViewModel ViewModel { get; }

        private readonly FlightsDashboardViewModel flightsViewModel;
        private readonly EmployeesDashboardViewModel employeesViewModel;
        private readonly AirportDashboardViewModel airportViewModel;

        public AirportAdminPage()
        {
            ViewModel = App.Services.GetRequiredService<AirportAdminViewModel>();
            flightsViewModel = App.Services.GetRequiredService<FlightsDashboardViewModel>();
            employeesViewModel = App.Services.GetRequiredService<EmployeesDashboardViewModel>();
            airportViewModel = App.Services.GetRequiredService<AirportDashboardViewModel>();

            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.Initialize();
            NavigateToSelectedSection();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AirportAdminViewModel.SelectedSection))
            {
                NavigateToSelectedSection();
            }
        }

        private void NavigateToSelectedSection()
        {
            switch (ViewModel.SelectedSection)
            {
                case AirportAdminSection.Flights:
                    if (ContentFrame.CurrentSourcePageType != typeof(FlightsDashboardPage))
                    {
                        ContentFrame.Navigate(typeof(FlightsDashboardPage), flightsViewModel);
                    }

                    HighlightSelectedButton(FlightsButton);
                    break;

                case AirportAdminSection.Employees:
                    if (ContentFrame.CurrentSourcePageType != typeof(EmployeesDashboardPage))
                    {
                        ContentFrame.Navigate(typeof(EmployeesDashboardPage), employeesViewModel);
                    }

                    HighlightSelectedButton(EmployeesButton);
                    break;

                case AirportAdminSection.AirportConfiguration:
                    if (ContentFrame.CurrentSourcePageType != typeof(AirportDashboardPage))
                    {
                        ContentFrame.Navigate(typeof(AirportDashboardPage), airportViewModel);
                    }

                    HighlightSelectedButton(AirportButton);
                    break;
            }
        }

        private void HighlightSelectedButton(Button selectedButton)
        {
            FlightsButton.Background = null;
            EmployeesButton.Background = null;
            AirportButton.Background = null;

            selectedButton.Background = new SolidColorBrush(Microsoft.UI.Colors.LightGray);
        }
    }
}
