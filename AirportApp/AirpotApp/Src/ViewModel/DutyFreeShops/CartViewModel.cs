using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Src.ViewModel.DutyFreeShops.Interface;

using UserSession = AirportLib.Domain.User.UserSession;

using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel.DutyFreeShops
{
    public partial class CartViewModel : INotifyPropertyChanged, ICartViewModel
    {
        private readonly ICartService cartService;
        private readonly IReservationService reservationService;
        private readonly UserSession session;
        private Reservation? currentReservation;

        private bool isReserved;
        private double overallTotal;

        public ObservableCollection<CartShopItem> CartShopItems { get; set; }

        public bool IsAdmin => session.IsAdmin;

        public bool IsReserved
        {
            get => isReserved;
            set
            {
                isReserved = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReserveButtonEnabled));
                OnPropertyChanged(nameof(IsCancelButtonVisible));
            }
        }

        public bool IsReserveButtonEnabled => !IsReserved;

        public bool IsCancelButtonVisible => IsReserved;

        public string OverallTotal => string.Format("${0:0.00}", overallTotal);

        public event PropertyChangedEventHandler? PropertyChanged;

        public CartViewModel(ICartService cartService, IReservationService reservationService, UserSession session)
        {
            if (session.IsAdmin)
            {
                throw new UnauthorizedAccessException("Admins are not allowed to view or enter the Cart.");
            }

            this.cartService = cartService;
            this.reservationService = reservationService;
            this.session = session;
            CartShopItems = new ObservableCollection<CartShopItem>();
            LoadCartItems();
            CheckExistingReservation();
        }

        public void Reload()
        {
            LoadCartItems();
        }

        public void ChangeQuantity(CartShopItem cartShopItem, int newQuantity)
        {
            RunSync(() => cartService.UpdateItemQuantityAsync(cartShopItem.CartItemId, cartShopItem.CartItemId, newQuantity));
            cartShopItem.Quantity = newQuantity;
            overallTotal = RunSync(() => cartService.GetCartTotalAsync(session.UserId));
            OnPropertyChanged(nameof(OverallTotal));
        }

        [RelayCommand]
        public void RemoveShopItem(CartShopItem cartShopItem)
        {
            RunSync(() => cartService.RemoveItemFromCartAsync(session.UserId, cartShopItem.CartItemId));
            CartShopItems.Remove(cartShopItem);
            overallTotal = RunSync(() => cartService.GetCartTotalAsync(session.UserId));
            OnPropertyChanged(nameof(OverallTotal));
        }

        [RelayCommand]
        public void EmptyCart()
        {
            RunSync(() => cartService.ClearCartAsync(session.UserId));
            CartShopItems.Clear();
            overallTotal = RunSync(() => cartService.GetCartTotalAsync(session.UserId));
            OnPropertyChanged(nameof(OverallTotal));
        }

        [RelayCommand]
        public void DecreaseQuantity(CartShopItem cartShopItem)
        {
            RunSync(() => cartService.DecreaseItemQuantityAsync(session.UserId, cartShopItem.CartItemId));
            Reload();
        }

        [RelayCommand]
        public void ReserveCart()
        {
            var cart = RunSync(() => cartService.GetCartByIdAsync(session.UserId));
            if (cart == null)
            {
                return;
            }

            var newReservation = new Reservation(cart, true, DateTime.Now);
            RunSync(() => reservationService.ReserveCartAsync(newReservation));
            currentReservation = newReservation;
            IsReserved = true;
        }

        [RelayCommand]
        public void CancelReservation()
        {
            if (currentReservation == null)
            {
                return;
            }

            RunSync(() => reservationService.CancelReservationAsync(currentReservation.Id));
            IsReserved = false;
            Reload();
        }

        public bool IsLastItem(CartShopItem cartShopItem)
        {
            return RunSync(() => cartService.IsLastCartItemAsync(session.UserId, cartShopItem.CartItemId));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CheckExistingReservation()
        {
            var cart = RunSync(() => cartService.GetCartByIdAsync(session.UserId));
            if (cart == null)
            {
                return;
            }

            var activeReservation = RunSync(() => reservationService.GetActiveReservationForCartAsync(cart.Id));
            if (activeReservation != null)
            {
                currentReservation = activeReservation;
                IsReserved = true;
            }
        }

        private void LoadCartItems()
        {
            CartShopItems.Clear();
            var cartItems = RunSync(() => cartService.GetCartItemsAsync(session.UserId));
            foreach (var existingCartItem in cartItems)
            {
                CartShopItems.Add(new CartShopItem
                {
                    CartItemId = existingCartItem.Id,
                    ShopItem = existingCartItem.ShopItem,
                    Quantity = existingCartItem.Quantity,
                });
            }

            overallTotal = RunSync(() => cartService.GetCartTotalAsync(session.UserId));
            OnPropertyChanged(nameof(OverallTotal));
        }

        private static T RunSync<T>(Func<Task<T>> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();

        private static void RunSync(Func<Task> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();
    }

    public partial class CartShopItem : INotifyPropertyChanged
    {
        private int quantity;
        public int CartItemId { get; set; }
        public ShopItem ShopItem { get; set; } = null!;

        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ItemTotalPrice));
            }
        }

        public string DisplayPrice
        {
            get
            {
                if (ShopItem != null)
                {
                    return string.Format("${0:0.00}", ShopItem.Price);
                }

                return "$0.00";
            }
        }

        public string ItemTotalPrice
        {
            get
            {
                if (ShopItem != null)
                {
                    return string.Format("${0:0.00}", Quantity * ShopItem.Price);
                }

                return "$0.00";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
