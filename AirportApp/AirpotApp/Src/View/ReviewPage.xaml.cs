using System;
using AirportApp.Src.View.Faq;
using AirportApp.Src.View.General;
using AirportApp.Src.View.Ticket;
using AirportApp.Src.ViewModel;
using AirportApp.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Review
{
    public sealed partial class ReviewPage : Page
    {
        public AddReviewViewModel ViewModel { get; }
        private readonly INavigationService navigationService;

        public ReviewPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetService<AddReviewViewModel>();
            navigationService = App.Services.GetRequiredService<INavigationService>();

            this.DataContext = ViewModel;
            ViewModel.AlertRequested += OnAlertRequested;

            this.Unloaded += (sender, eventArguments) =>
            {
                ViewModel.AlertRequested -= OnAlertRequested;
            };
        }

        private async void OnAlertRequested(object? sender, (string Title, string Message) alertEventArguments)
        {
            var dialog = new ErrorDialog(alertEventArguments.Message, alertEventArguments.Title)
            {
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private async void NavigateToTicketsView_Click(object sender, RoutedEventArgs arguments)
        {
            var button = sender as Button;

            navigationService.NavigateTo(typeof(ComplaintTicketPage));
        }
    }
}

