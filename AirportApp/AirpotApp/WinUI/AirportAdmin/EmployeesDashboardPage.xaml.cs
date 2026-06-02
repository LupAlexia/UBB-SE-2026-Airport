using AirportApp.Src.ViewModel;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI.AirportAdmin
{
    public sealed partial class EmployeesDashboardPage : Page
    {
        public EmployeesDashboardViewModel? ViewModel { get; private set; }

        public EmployeesDashboardPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is EmployeesDashboardViewModel vm)
            {
                ViewModel = vm;
                DataContext = ViewModel;
                ViewModel.LoadData();
            }
        }
    }
}
