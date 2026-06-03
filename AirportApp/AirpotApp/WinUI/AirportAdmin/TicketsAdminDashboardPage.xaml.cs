using System;
using System.Linq;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.ViewModel;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI.AirportAdmin
{
    public sealed partial class TicketsAdminDashboardPage : Page
    {
        public ComplaintTicketViewModel? ViewModel { get; private set; }

        public TicketsAdminDashboardPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is ComplaintTicketViewModel vm)
            {
                ViewModel = vm;
                DataContext = ViewModel;
            }
        }

        private async void EditTicketStatus_Click(object sender, RoutedEventArgs arguments)
        {
            if (ViewModel == null || sender is not Button button || button.Tag is not int ticketId)
            {
                return;
            }

            var ticket = ViewModel.FilteredTicketsForDisplay.FirstOrDefault(ticketDto => ticketDto.ticketId == ticketId);
            if (ticket == null)
            {
                return;
            }

            var primaryButtonStyle = new Style(typeof(Button));
            primaryButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x2B, 0xB8, 0xC0))));
            primaryButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.White)));
            primaryButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x2B, 0xB8, 0xC0))));
            primaryButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(1)));
            primaryButtonStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(5)));

            var closeButtonStyle = new Style(typeof(Button));
            closeButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xE5, 0xE7, 0xEB))));
            closeButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            closeButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xE5, 0xE7, 0xEB))));
            closeButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(1)));
            closeButtonStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(5)));

            var dialog = new ContentDialog
            {
                Title = $"Edit CurrentStatus for Ticket #{ticket.ticketId}",
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot,
                RequestedTheme = ElementTheme.Light,
                PrimaryButtonStyle = primaryButtonStyle,
                CloseButtonStyle = closeButtonStyle
            };

            var combo = new ComboBox
            {
                Width = 200,
                Margin = new Thickness(0, 20, 0, 0),
                RequestedTheme = ElementTheme.Light,
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };

            foreach (var status in Enum.GetValues(typeof(ComplaintTicketStatusEnum)).Cast<ComplaintTicketStatusEnum>())
            {
                combo.Items.Add(status.ToString());
            }

            combo.SelectedItem = ticket.currentStatus.ToString();
            dialog.Content = combo;

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && combo.SelectedItem is string selectedStr
                && Enum.TryParse<ComplaintTicketStatusEnum>(selectedStr, out var newStatus))
            {
                await ViewModel.UpdateStatusAsync(ticket.ticketId, newStatus);
            }
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs arguments)
        {
            if (ViewModel == null || sender is not ComboBox combo || combo.SelectedItem is not ComboBoxItem selected || selected.Tag == null)
            {
                return;
            }

            ViewModel.SelectedFilterString = selected.Tag.ToString()!;
        }
    }
}
