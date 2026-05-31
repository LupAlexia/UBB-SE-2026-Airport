using AirportApp.Services.Interfaces;
using AirportApp.Src.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.General
{
    public sealed partial class UpperBarControl : UserControl
    {
        public AirportApp.Src.ViewModel.UpperBarViewModel ViewModel { get; }
        private readonly INavigationService navigationService;

        public UpperBarControl()
        {
            this.InitializeComponent();

            ViewModel = App.Services.GetService<AirportApp.Src.ViewModel.UpperBarViewModel>();
            this.DataContext = ViewModel;
            navigationService = App.Services.GetRequiredService<INavigationService>();
        }

        private DependencyObject FindParentFrame()
        {
            DependencyObject parent = this.Parent;
            while (parent != null && !(parent is Frame))
            {
                parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        public void OnChatRequested(object sender, RoutedEventArgs arguments)
        {
            navigationService.NavigateTo(ViewModel.ChatPageType);
        }

        public void OnLandingRequested(object sender, RoutedEventArgs arguments)
        {
            navigationService.NavigateTo(ViewModel.LandingPageType);
        }

        public void OnFAQRequested(object sender, RoutedEventArgs arguments)
        {
            navigationService.NavigateTo(ViewModel.FAQPageType);
        }

        public void OnTicketsRequested(object sender, RoutedEventArgs arguments)
        {
            navigationService.NavigateTo(ViewModel.TicketsPageType);
        }

        public void OnReviewsRequested(object sender, RoutedEventArgs arguments)
        {
            navigationService.NavigateTo(ViewModel.ReviewsPageType);
        }

        public void OnHomeRequested(object sender, RoutedEventArgs arguments)
        {
            navigationService.NavigateTo(ViewModel.HomePageType);
        }
    }
}
