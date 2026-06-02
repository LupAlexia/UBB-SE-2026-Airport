using AirportApp.Src.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI
{
    public sealed partial class StaffPage : Page
    {
        public StaffPageViewModel ViewModel { get; }

        public StaffPage()
        {
            ViewModel = App.Services.GetRequiredService<StaffPageViewModel>();
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int employeeId)
            {
                ViewModel.Initialize(employeeId);
            }
        }
    }
}
