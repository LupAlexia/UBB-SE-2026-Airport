using AirportApp.Src.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI
{
    public sealed partial class CompanyPage : Page
    {
        private const string DefaultErrorTitle = "Error Saving Flight";
        private const string DefaultErrorContent = "Please ensure all fields are filled correctly: ";
        private const string DefaultErrorCloseButtonText = "Ok";

        public CompanyViewModel ViewModel { get; }

        public CompanyPage()
        {
            ViewModel = App.Services.GetRequiredService<CompanyViewModel>();
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int companyId)
            {
                ViewModel.RefreshGatesList();
                ViewModel.RefreshRunwaysList();
                ViewModel.InitializeCompanyDashboard(companyId);
            }
        }

        private async void AddFlightButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetInputFields();
            await AddFlightDialog.ShowAsync();
        }

        private async void AddFlightDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                ViewModel.AddFlightFromInputs();
            }
            catch (Exception ex)
            {
                args.Cancel = true;
                sender.Hide();
                var errorDialog = new ContentDialog
                {
                    Title = DefaultErrorTitle,
                    Content = DefaultErrorContent + ex.Message,
                    CloseButtonText = DefaultErrorCloseButtonText,
                    XamlRoot = XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }
}
