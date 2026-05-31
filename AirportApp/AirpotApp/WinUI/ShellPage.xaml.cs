using AirportApp.Services.Interfaces;
using AirportApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; }

        public ShellPage()
        {
            InitializeComponent();

            var navigationService = App.Services.GetRequiredService<INavigationService>();
            navigationService.Initialize(ContentFrame);

            ViewModel = App.Services.GetRequiredService<ShellViewModel>();
            DataContext = ViewModel;

            ViewModel.NavigateToAirportCommand.Execute(null);
        }
    }
}
