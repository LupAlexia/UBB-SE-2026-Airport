using System.ComponentModel;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.ViewModel.DutyFreeShops.Interface;
using AirportApp.WinUI.DutyFreeShops.CartPage;
using AirportApp.WinUI.DutyFreeShops.ShopItemsPage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.WinUI.DutyFreeShops.ItemDetailsPage
{
    public sealed partial class DutyFreeItemDetailsPage : Page
    {
        public IItemDetailsViewModel? ViewModel { get; private set; }

        public DutyFreeItemDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not ItemDetailsNavArgs args)
            {
                return;
            }

            var viewModelFactory = App.Services.GetRequiredService<Func<ShopItem, Shop, IItemDetailsViewModel>>();
            ViewModel = viewModelFactory(args.Item, args.Shop);
            DataContext = ViewModel;

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.AddedToCartSuccessfully += OnAddedToCartSuccessfully;
            ViewModel.ErrorOccurred += OnErrorOccurred;

            PopulateDisplayFields();
            SetMode(ViewModel.IsAdmin);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.AddedToCartSuccessfully -= OnAddedToCartSuccessfully;
                ViewModel.ErrorOccurred -= OnErrorOccurred;
            }
        }

        private void PopulateDisplayFields()
        {
            if (ViewModel == null)
            {
                return;
            }

            TitleText.Text = ViewModel.Name;
            DescText.Text = ViewModel.Description;
            PriceText.Text = ViewModel.FormattedPrice;
            StockUnitsRun.Text = $"{ViewModel.Stock} units";
            QuantityBox.Text = ViewModel.Quantity.ToString();

            if (ViewModel.IsAdmin)
            {
                AdminNameBox.Text = ViewModel.EditName;
                AdminDescBox.Text = ViewModel.EditDescription;
                AdminPriceBox.Text = ViewModel.EditPrice;
                AdminStockBox.Text = ViewModel.EditStock;
                AdminStatusText.Text = string.Empty;
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (e.PropertyName == nameof(IItemDetailsViewModel.Quantity))
            {
                var quantityText = ViewModel.Quantity.ToString();
                if (QuantityBox.Text != quantityText)
                {
                    QuantityBox.Text = quantityText;
                }
            }
            else if (e.PropertyName == nameof(IItemDetailsViewModel.StatusMessage))
            {
                AdminStatusText.Text = ViewModel.StatusMessage;
            }
            else if (e.PropertyName == nameof(IItemDetailsViewModel.Name))
            {
                TitleText.Text = ViewModel.Name;
                DescText.Text = ViewModel.Description;
                PriceText.Text = ViewModel.FormattedPrice;
                StockUnitsRun.Text = $"{ViewModel.Stock} units";
            }
        }

        private async void OnErrorOccurred(object? sender, string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = Root.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async void OnAddedToCartSuccessfully(object? sender, EventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Added to cart",
                Content = $"Quantity: {ViewModel?.Quantity}",
                CloseButtonText = "OK",
                XamlRoot = Root.XamlRoot
            };
            await dialog.ShowAsync();
            NavigateToShop();
        }

        private void QuantityMinus_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.DecrementQuantityCommand.Execute(null);
        }

        private void QuantityPlus_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.IncrementQuantityCommand.Execute(null);
        }

        private void QuantityBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel?.SetQuantityFromText(QuantityBox.Text);
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.AddToCartCommand.Execute(null);
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DutyFreeCartPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToShop();
        }

        private void NavigateToShop()
        {
            if (ViewModel?.CurrentShop != null)
            {
                Frame.Navigate(typeof(DutyFreeShopItemsPage), ViewModel.CurrentShop);
            }
        }

        private void SetMode(bool isAdmin)
        {
            ViewCartButton.IsEnabled = !isAdmin;

            SolidColorBrush cartForeground = isAdmin
                ? (SolidColorBrush)Root.Resources["DisabledCartBrush"]
                : (SolidColorBrush)Root.Resources["DarkText"];

            ViewCartIcon.Foreground = cartForeground;
            ViewCartText.Foreground = cartForeground;

            ClientActionsPanel.Visibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
            AdminActionsPanel.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AdminSave_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.EditName = AdminNameBox.Text;
            ViewModel.EditDescription = AdminDescBox.Text;
            ViewModel.EditPrice = AdminPriceBox.Text;
            ViewModel.EditStock = AdminStockBox.Text;
            ViewModel.SaveChangesCommand.Execute(null);
        }
    }
}
