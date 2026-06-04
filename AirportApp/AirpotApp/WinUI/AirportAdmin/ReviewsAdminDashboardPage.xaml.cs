using AirportApp.Src.ViewModel;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI.AirportAdmin
{
    public sealed partial class ReviewsAdminDashboardPage : Page
    {
        public AllReviewsViewModel? ViewModel { get; private set; }

        public ReviewsAdminDashboardPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is AllReviewsViewModel vm)
            {
                ViewModel = vm;
                DataContext = ViewModel;
                await ViewModel.LoadDataAsync();
            }
        }
    }
}
