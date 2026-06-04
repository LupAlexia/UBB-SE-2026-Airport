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
        private readonly FAQViewModel faqViewModel;
        private readonly ComplaintTicketViewModel ticketsViewModel;
        private readonly AllReviewsViewModel reviewsViewModel;

        public AirportAdminPage()
        {
            ViewModel = App.Services.GetRequiredService<AirportAdminViewModel>();
            flightsViewModel = App.Services.GetRequiredService<FlightsDashboardViewModel>();
            employeesViewModel = App.Services.GetRequiredService<EmployeesDashboardViewModel>();
            airportViewModel = App.Services.GetRequiredService<AirportDashboardViewModel>();
            faqViewModel = App.Services.GetRequiredService<FAQViewModel>();
            ticketsViewModel = App.Services.GetRequiredService<ComplaintTicketViewModel>();
            reviewsViewModel = App.Services.GetRequiredService<AllReviewsViewModel>();

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

                case AirportAdminSection.FAQ:
                    if (ContentFrame.CurrentSourcePageType != typeof(FAQAdminDashboardPage))
                    {
                        ContentFrame.Navigate(typeof(FAQAdminDashboardPage), faqViewModel);
                    }

                    HighlightSelectedButton(FAQButton);
                    break;

                case AirportAdminSection.Tickets:
                    if (ContentFrame.CurrentSourcePageType != typeof(TicketsAdminDashboardPage))
                    {
                        ContentFrame.Navigate(typeof(TicketsAdminDashboardPage), ticketsViewModel);
                    }

                    HighlightSelectedButton(TicketsButton);
                    break;

                case AirportAdminSection.Reviews:
                    if (ContentFrame.CurrentSourcePageType != typeof(ReviewsAdminDashboardPage))
                    {
                        ContentFrame.Navigate(typeof(ReviewsAdminDashboardPage), reviewsViewModel);
                    }

                    HighlightSelectedButton(ReviewsButton);
                    break;
            }
        }

        private void HighlightSelectedButton(Button selectedButton)
        {
            FlightsButton.Background = null;
            EmployeesButton.Background = null;
            AirportButton.Background = null;
            FAQButton.Background = null;
            TicketsButton.Background = null;
            ReviewsButton.Background = null;

            selectedButton.Background = new SolidColorBrush(Microsoft.UI.Colors.LightGray);
        }
    }
}
