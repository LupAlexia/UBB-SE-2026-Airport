using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Src.ViewModel;

using UserSession = AirportLib.Domain.User.UserSession;

using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel
{
    public class ItemDetailsViewModel : IItemDetailsViewModel
    {
        private const int MinimumQuantity = 1;
        private const int MaximumQuantity = 99;

        private readonly ICartService cartService;
        private readonly IShopItemService shopItemService;
        private readonly UserSession session;
        private readonly ShopItem item;

        private int quantity;
        private string editName;
        private string editDescription;
        private string editPrice;
        private string editStock;
        private string statusMessage = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? AddedToCartSuccessfully;
        public event EventHandler<string>? ErrorOccurred;

        public string Name => item.Name;
        public string Description => item.Description;
        public string FormattedPrice => $"{item.Price:C}";
        public int Stock => item.Quantity;
        public string Photo => item.Photo;
        public bool IsAdmin => session.IsAdmin;
        public Shop CurrentShop { get; }

        public int Quantity
        {
            get => quantity;
            private set
            {
                if (quantity == value)
                {
                    return;
                }

                quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public string EditName
        {
            get => editName;
            set { editName = value; OnPropertyChanged(nameof(EditName)); }
        }

        public string EditDescription
        {
            get => editDescription;
            set { editDescription = value; OnPropertyChanged(nameof(EditDescription)); }
        }

        public string EditPrice
        {
            get => editPrice;
            set { editPrice = value; OnPropertyChanged(nameof(EditPrice)); }
        }

        public string EditStock
        {
            get => editStock;
            set { editStock = value; OnPropertyChanged(nameof(EditStock)); }
        }

        public string StatusMessage
        {
            get => statusMessage;
            private set { statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        public ICommand AddToCartCommand { get; }
        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public ItemDetailsViewModel(
            ICartService cartService,
            IShopItemService shopItemService,
            UserSession session,
            ShopItem item,
            Shop currentShop)
        {
            this.cartService = cartService;
            this.shopItemService = shopItemService;
            this.session = session;
            this.item = item;
            CurrentShop = currentShop;

            editName = item.Name;
            editDescription = item.Description;
            editPrice = item.Price.ToString(CultureInfo.InvariantCulture);
            editStock = item.Quantity.ToString(CultureInfo.InvariantCulture);
            quantity = MinimumQuantity;

            AddToCartCommand = new RelayCommand(AddToCart);
            IncrementQuantityCommand = new RelayCommand(IncrementQuantity);
            DecrementQuantityCommand = new RelayCommand(DecrementQuantity);
            SaveChangesCommand = new RelayCommand(SaveChanges);
        }

        public void SetQuantityFromText(string text)
        {
            if (int.TryParse(text, out int parsedQuantity))
            {
                Quantity = LimitQuantityToValidRange(parsedQuantity);
            }
            else
            {
                OnPropertyChanged(nameof(Quantity));
            }
        }

        private void IncrementQuantity()
        {
            Quantity = LimitQuantityToValidRange(Quantity + 1);
        }

        private void DecrementQuantity()
        {
            Quantity = LimitQuantityToValidRange(Quantity - 1);
        }

        private void AddToCart()
        {
            Cart cart = RunSync(() => cartService.GetOrCreateCartAsync(session.UserId));

            try
            {
                RunSync(() => cartService.AddItemToCartAsync(cart.Id, item.Id, quantity));
            }
            catch (InvalidOperationException exception)
            {
                ErrorOccurred?.Invoke(this, exception.Message);
                return;
            }

            AddedToCartSuccessfully?.Invoke(this, EventArgs.Empty);
        }

        private void SaveChanges()
        {
            string trimmedName = (editName ?? string.Empty).Trim();
            string trimmedDescription = editDescription ?? string.Empty;
            string trimmedPriceText = (editPrice ?? string.Empty).Trim();

            if (!int.TryParse(editStock, out int parsedStock))
            {
                StatusMessage = "Error: Stock must be a number.";
                return;
            }

            if (!float.TryParse(trimmedPriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedPrice))
            {
                StatusMessage = "Error: Price must be a number.";
                return;
            }

            item.Name = trimmedName;
            item.Description = trimmedDescription;
            item.Price = parsedPrice;
            item.Quantity = parsedStock;

            try
            {
                RunSync(() => shopItemService.UpdateShopItemAsync(
                    new ShopItem(item.Id, item.Quantity, item.Price, item.Shop, item.Photo, item.Name, item.Description)));

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(FormattedPrice));
                OnPropertyChanged(nameof(Stock));

                StatusMessage = "Saved";
            }
            catch (ArgumentException exception)
            {
                StatusMessage = exception.Message;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int LimitQuantityToValidRange(int qty)
        {
            return Math.Max(MinimumQuantity, Math.Min(MaximumQuantity, qty));
        }

        private static T RunSync<T>(Func<Task<T>> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();

        private static void RunSync(Func<Task> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();
    }
}
