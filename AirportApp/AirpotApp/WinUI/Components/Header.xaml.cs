using AirportApp.Src.ViewModel;
using AirportApp.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI.Components
{
    public sealed partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationService = App.Services.GetRequiredService<INavigationService>();
            navigationService.NavigateTo(typeof(HomePage));
        }
    }
}
