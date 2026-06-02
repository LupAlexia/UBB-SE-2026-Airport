using AirportApp.Src.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI.StaffLogin
{
    public sealed partial class StaffLoginPage : Page
    {
        public StaffLoginViewModel ViewModel { get; }

        public StaffLoginPage()
        {
            ViewModel = App.Services.GetRequiredService<StaffLoginViewModel>();
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
