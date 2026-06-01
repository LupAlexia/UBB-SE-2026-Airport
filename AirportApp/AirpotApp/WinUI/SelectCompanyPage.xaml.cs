using AirportApp.Src.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI
{
    public sealed partial class SelectCompanyPage : Page
    {
        public SelectCompanyViewModel ViewModel { get; }

        public SelectCompanyPage()
        {
            ViewModel = App.Services.GetRequiredService<SelectCompanyViewModel>();
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
