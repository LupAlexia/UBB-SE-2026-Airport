using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel
{
    public interface ICartViewModel : INotifyPropertyChanged
    {
        bool IsCancelButtonVisible { get; }
        IRelayCommand ReserveCartCommand { get; }
        IRelayCommand CancelReservationCommand { get; }
        ObservableCollection<CartShopItem> CartShopItems { get; set; }
        bool IsAdmin { get; }
        bool IsReserveButtonEnabled { get; }
        bool IsReserved { get; set; }
        string OverallTotal { get; }

        void CancelReservation();
        void ChangeQuantity(CartShopItem item, int newQuantity);
        void EmptyCart();
        void Reload();
        void RemoveShopItem(CartShopItem item);
        void ReserveCart();
        void DecreaseQuantity(CartShopItem item);
        bool IsLastItem(CartShopItem item);
    }
}
