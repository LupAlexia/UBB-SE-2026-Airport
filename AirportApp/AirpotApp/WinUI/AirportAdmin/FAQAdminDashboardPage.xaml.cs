using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Services.Interfaces;
using AirportApp.Src.View.Faq;
using AirportApp.Src.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI.AirportAdmin
{
    public sealed partial class FAQAdminDashboardPage : Page
    {
        public FAQViewModel? ViewModel { get; private set; }

        private readonly INavigationService navigationService;
        private int currentPersonId;

        public FAQAdminDashboardPage()
        {
            InitializeComponent();
            navigationService = App.Services.GetRequiredService<INavigationService>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not FAQViewModel vm)
            {
                return;
            }

            ViewModel = vm;
            ViewModel.IsAdmin = true;
            DataContext = ViewModel;

            var app = (App)App.Current;
            if (app.Employee != null)
            {
                currentPersonId = app.Employee.Id;
            }
            else if (app.User != null)
            {
                currentPersonId = app.User.RetrieveUniqueDatabaseIdentifierForBot();
            }

            await ViewModel.LoadFAQAsync();
        }

        private void OpenFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (sender is Button button && button.DataContext is FAQEntryDTO faq)
            {
                ViewModel?.ToggleFAQ(faq);
                ScrollToMiddleIfExpanded(faq);
            }
        }

        private void AccordionHeader_Click(object sender, RoutedEventArgs arguments)
        {
            if (sender is Button button && button.DataContext is FAQEntryDTO faq)
            {
                ViewModel?.ToggleFAQ(faq);
                ScrollToMiddleIfExpanded(faq);
            }
        }

        private void AllQuestionsButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel?.SetCategory(FAQCategoryEnum.All);
            SetCategoryUI(AllQuestionsButton);
        }

        private void CheckInButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel?.SetCategory(FAQCategoryEnum.CheckIn);
            SetCategoryUI(CheckInButton);
        }

        private void ParkingButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel?.SetCategory(FAQCategoryEnum.Parking);
            SetCategoryUI(ParkingButton);
        }

        private void BaggageButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel?.SetCategory(FAQCategoryEnum.Baggage);
            SetCategoryUI(BaggageButton);
        }

        private void TicketButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel?.SetCategory(FAQCategoryEnum.Tickets);
            SetCategoryUI(FaqCategoryTicketsButton);
        }

        private void FacilitiesButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel?.SetCategory(FAQCategoryEnum.Facilities);
            SetCategoryUI(FacilitiesButton);
        }

        private void AddFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (ViewModel == null)
            {
                return;
            }

            navigationService.NavigateTo(typeof(FAQAddEditPage), ViewModel.BuildAddNavigationData(currentPersonId));
        }

        private void EditFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (ViewModel?.SelectedFAQEntry == null)
            {
                return;
            }

            navigationService.NavigateTo(typeof(FAQAddEditPage), ViewModel.BuildNavigationData(currentPersonId));
        }

        private async void DeleteFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (ViewModel?.SelectedFAQEntry == null)
            {
                return;
            }

            var faq = ViewModel.SelectedFAQEntry;

            var dialog = new ContentDialog
            {
                Title = "Delete FAQ",
                Content = $"Are you sure you want to delete \"{faq.Question}\"?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.DeleteFAQEntryAsync(faq);
                ViewModel.SelectedFAQEntry = null;
            }
        }

        private void ScrollToMiddleIfExpanded(FAQEntryDTO faq)
        {
            if (!faq.IsExpanded)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                var container = AllQuestionsList.ContainerFromItem(faq) as FrameworkElement;
                container?.StartBringIntoView(new BringIntoViewOptions
                {
                    VerticalAlignmentRatio = 0.5,
                    AnimationDesired = true
                });
            });
        }

        private void SetCategoryUI(Button selected)
        {
            var normal = (Style)Resources["CategoryButtonStyle"];
            var active = (Style)Resources["SelectedCategoryButtonStyle"];

            AllQuestionsButton.Style = normal;
            CheckInButton.Style = normal;
            ParkingButton.Style = normal;
            BaggageButton.Style = normal;
            FaqCategoryTicketsButton.Style = normal;
            FacilitiesButton.Style = normal;

            selected.Style = active;
        }
    }
}
