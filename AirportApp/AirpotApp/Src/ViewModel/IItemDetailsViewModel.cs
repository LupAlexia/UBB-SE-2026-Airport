using System.ComponentModel;
using System.Windows.Input;

using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.ViewModel
{
    public interface IItemDetailsViewModel : INotifyPropertyChanged
    {
        string Name { get; }
        string Description { get; }
        string FormattedPrice { get; }
        int Stock { get; }
        string Photo { get; }
        bool IsAdmin { get; }
        Shop CurrentShop { get; }

        int Quantity { get; }
        void SetQuantityFromText(string text);

        string EditName { get; set; }
        string EditDescription { get; set; }
        string EditPrice { get; set; }
        string EditStock { get; set; }
        string StatusMessage { get; }

        ICommand AddToCartCommand { get; }
        ICommand IncrementQuantityCommand { get; }
        ICommand DecrementQuantityCommand { get; }
        ICommand SaveChangesCommand { get; }

        event EventHandler AddedToCartSuccessfully;
        event EventHandler<string> ErrorOccurred;
    }
}
