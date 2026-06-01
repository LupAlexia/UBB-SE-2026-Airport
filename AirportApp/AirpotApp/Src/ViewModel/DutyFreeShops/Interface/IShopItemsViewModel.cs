using System.Collections.ObjectModel;
using System.ComponentModel;

using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.ViewModel.DutyFreeShops.Interface
{
    public interface IShopItemsViewModel : INotifyPropertyChanged
    {
        bool IsAdmin { get; }
        bool CanAddItem { get; }
        bool IsCartEnabled { get; }
        ObservableCollection<ShopItem> Items { get; }

        void LoadItems();
        void AddItem(string name, string description, string priceText, string quantityText, string imagePath);
        void UpdateItem(ShopItem item, string name, string description, string priceText, string quantityText, string imagePath);
        void DeleteItem(ShopItem item);
        void AddToCart(ShopItem item, int quantity);
        void Search(string query);
        void SortByName();
        void SortByPrice();
    }
}
