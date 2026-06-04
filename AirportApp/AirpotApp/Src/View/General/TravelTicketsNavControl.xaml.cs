using AirportApp.Services.Interfaces;
using AirportApp.Src.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AirportApp.Src.View.General
{
    public sealed partial class TravelTicketsNavControl : UserControl
    {
        private const string SearchSection = "Search";
        private const string MyTicketsSection = "MyTickets";
        private const string MembershipsSection = "Memberships";

        private readonly INavigationService navigationService;

        public TravelTicketsNavControl()
        {
            this.InitializeComponent();
            navigationService = App.Services.GetRequiredService<INavigationService>();
            UpdateActiveStates();
        }

        public static readonly DependencyProperty SelectedSectionProperty =
            DependencyProperty.Register(
                nameof(SelectedSection),
                typeof(string),
                typeof(TravelTicketsNavControl),
                new PropertyMetadata(SearchSection, OnSelectedSectionChanged));

        public string SelectedSection
        {
            get => (string)GetValue(SelectedSectionProperty);
            set => SetValue(SelectedSectionProperty, value);
        }

        private static void OnSelectedSectionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((TravelTicketsNavControl)dependencyObject).UpdateActiveStates();
        }

        private void UpdateActiveStates()
        {
            SetTabState(SearchTabButton, SelectedSection == SearchSection);
            SetTabState(MyTicketsTabButton, SelectedSection == MyTicketsSection);
            SetTabState(MembershipsTabButton, SelectedSection == MembershipsSection);
        }

        private void SetTabState(Button tabButton, bool isActive)
        {
            tabButton.Background = (Brush)Resources[isActive ? "TabActiveBackground" : "TabInactiveBackground"];
            tabButton.Foreground = (Brush)Resources[isActive ? "TabActiveForeground" : "TabInactiveForeground"];
        }

        private void OnSearchRequested(object sender, RoutedEventArgs eventArgs)
        {
            if (SelectedSection == SearchSection)
            {
                return;
            }
            navigationService.NavigateTo(typeof(FlightSearchPage));
        }

        private void OnMyTicketsRequested(object sender, RoutedEventArgs eventArgs)
        {
            if (SelectedSection == MyTicketsSection)
            {
                return;
            }
            navigationService.NavigateTo(typeof(DashboardPage));
        }

        private void OnMembershipsRequested(object sender, RoutedEventArgs eventArgs)
        {
            if (SelectedSection == MembershipsSection)
            {
                return;
            }
            navigationService.NavigateTo(typeof(MembershipsPage));
        }

        private void OnHomeRequested(object sender, RoutedEventArgs eventArgs)
        {
            navigationService.NavigateTo(typeof(UserHomePage));
        }
    }
}
