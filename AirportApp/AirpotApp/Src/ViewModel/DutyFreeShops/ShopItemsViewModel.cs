using System.ComponentModel;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Src.ViewModel.DutyFreeShops.Interface;

using System.Collections.ObjectModel;

using UserSession = AirportLib.Domain.User.UserSession;

namespace AirportApp.Src.ViewModel.DutyFreeShops
{
    public class ShopItemsViewModel : IShopItemsViewModel
    {
        private readonly IShopItemService shopItemService;
        private readonly ICartService cartService;
        private readonly UserSession session;
        private readonly Shop currentShop;

        public bool IsAdmin => session.IsAdmin;
        public bool CanAddItem => session.IsAdmin;
        public bool IsCartEnabled => !session.IsAdmin;

        public ObservableCollection<ShopItem> Items { get; } = new ObservableCollection<ShopItem>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ShopItemsViewModel(IShopItemService shopItemService, ICartService cartService, UserSession session, Shop currentShop)
        {
            this.shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.currentShop = currentShop ?? throw new ArgumentNullException(nameof(currentShop));
            LoadItems();
        }

        public void LoadItems()
        {
            ReplaceItems(RunSync(() => shopItemService.GetItemsByShopIdAsync(currentShop.Id)));
        }

        public void AddItem(string name, string description, string priceText, string quantityText, string imagePath)
        {
            if (!float.TryParse(priceText, out float price))
            {
                throw new ArgumentException("Price is not a valid number.");
            }

            if (!int.TryParse(quantityText, out int quantity))
            {
                throw new ArgumentException("Quantity is not a valid number.");
            }

            RunSync(() => shopItemService.AddShopItemAsync(new ShopItem(quantity, price, currentShop, imagePath, name, description)));
            LoadItems();
        }

        public void UpdateItem(ShopItem item, string name, string description, string priceText, string quantityText, string imagePath)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!float.TryParse(priceText, out float price))
            {
                throw new ArgumentException("Price is not a valid number.");
            }

            if (!int.TryParse(quantityText, out int quantity))
            {
                throw new ArgumentException("Quantity is not a valid number.");
            }

            RunSync(() => shopItemService.UpdateShopItemAsync(new ShopItem(item.Id, quantity, price, item.Shop, imagePath, name, description)));
            LoadItems();
        }

        public void DeleteItem(ShopItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            RunSync(() => shopItemService.RemoveShopItemAsync(item.Id));
            LoadItems();
        }

        public void AddToCart(ShopItem item, int quantity)
        {
            var cart = RunSync(() => cartService.GetOrCreateCartAsync(session.UserId));
            RunSync(() => cartService.AddItemToCartAsync(cart.Id, item.Id, quantity));
        }

        public void SortByName()
        {
            ReplaceItems(RunSync(() => shopItemService.GetItemsSortedAlphabeticallyAsync(currentShop)));
        }

        public void SortByPrice()
        {
            ReplaceItems(RunSync(() => shopItemService.GetItemsSortedByPriceAsync(currentShop)));
        }

        public void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                LoadItems();
                return;
            }

            ReplaceItems(RunSync(() => shopItemService.SearchItemsByNameAsync(currentShop.Id, query)));
        }

        private void ReplaceItems(IEnumerable<ShopItem> items)
        {
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        private static T RunSync<T>(Func<Task<T>> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();

        private static void RunSync(Func<Task> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();
    }
}
