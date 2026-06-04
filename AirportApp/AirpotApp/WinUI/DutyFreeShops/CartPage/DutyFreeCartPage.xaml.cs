using AirportApp.Src.ViewModel;
using AirportApp.WinUI.DutyFreeShops.ShopPage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.WinUI.DutyFreeShops.CartPage
{
    public sealed partial class DutyFreeCartPage : Page
    {
        public ICartViewModel ViewModel { get; }

        public DutyFreeCartPage()
        {
            ViewModel = App.Services.GetRequiredService<ICartViewModel>();
            InitializeComponent();
        }

        public Visibility BoolToVisibility(bool value) =>
            value ? Visibility.Visible : Visibility.Collapsed;

        private async void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CartShopItem cartShopItem)
            {
                if (ViewModel.IsLastItem(cartShopItem))
                {
                    await ShowDeleteConfirmationAsync(cartShopItem);
                }
                else
                {
                    ViewModel.DecreaseQuantity(cartShopItem);
                }
            }
        }

        private async Task ShowDeleteConfirmationAsync(CartShopItem cartShopItem)
        {
            var dialog = new ContentDialog
            {
                Title = "Delete Item",
                Content = "Are you sure you want to delete this item?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                ViewModel.RemoveShopItem(cartShopItem);
            }
        }

        private async void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CartShopItem cartShopItem)
            {
                try
                {
                    ViewModel.ChangeQuantity(cartShopItem, cartShopItem.Quantity + 1);
                }
                catch (InvalidOperationException exception)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Cannot increase quantity",
                        Content = exception.Message,
                        CloseButtonText = "OK",
                        XamlRoot = XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void EmptyCart_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CartShopItems.Count == 0)
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = "Empty Cart",
                Content = "Are you sure you want to remove all items from your cart?",
                PrimaryButtonText = "Empty Cart",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                ViewModel.EmptyCart();
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Leave Cart",
                Content = "Are you sure you want to go back to the shop page?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                Frame.Navigate(typeof(DutyFreeShopPage));
            }
        }
    }
}
