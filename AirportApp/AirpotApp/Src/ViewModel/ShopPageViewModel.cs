using System.ComponentModel;
using System.Runtime.CompilerServices;

using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Src.ViewModel;

using System.Collections.ObjectModel;

using UserSession = AirportLib.Domain.User.UserSession;

using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel
{
    public partial class ShopPageViewModel : IShopPageViewModel
    {
        private readonly IShopService shopService;
        private readonly IManagerService managerService;
        private readonly UserSession session;

        private const double ClientCartOpacity = 1.0;
        private const double AdminCartOpacity = 0.4;

        public ObservableCollection<Shop> Shops { get; } = new ObservableCollection<Shop>();

        public bool IsAdmin => session.IsAdmin;
        public bool CanAddShop => session.IsAdmin;
        public bool IsCartEnabled => !session.IsAdmin;
        public double CartOpacity => session.IsAdmin ? AdminCartOpacity : ClientCartOpacity;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ShopPageViewModel(IShopService shopService, IManagerService managerService, UserSession session)
        {
            this.shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            this.managerService = managerService ?? throw new ArgumentNullException(nameof(managerService));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            LoadShops();
        }

        public void AddShop(string name, string type)
        {
            var manager = RunSync(() => managerService.GetManagerByIdAsync(session.UserId));
            if (manager == null)
            {
                throw new InvalidOperationException("Manager not found.");
            }

            RunSync(() => shopService.AddShopAsync(new Shop(name, type, manager)));
            LoadShops();
        }

        public void EditShop(Shop shop, string newName, string newType)
        {
            var manager = RunSync(() => managerService.GetManagerByIdAsync(session.UserId));
            if (manager == null)
            {
                throw new InvalidOperationException("Manager not found.");
            }

            RunSync(() => shopService.UpdateShopAsync(new Shop(shop.Id, newName, newType, manager)));
            LoadShops();
        }

        [RelayCommand]
        public void DeleteShop(Shop shop)
        {
            RunSync(() => shopService.DeleteShopAsync(shop.Id));
            LoadShops();
        }

        [RelayCommand]
        public void Search(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                LoadShops();
                return;
            }

            var results = RunSync(() => shopService.SearchByNameAsync(query));
            ReplaceShops(results);
        }

        [RelayCommand]
        public void SortByReviews()
        {
            // Sorting by reviews is not supported with the current service interface.
            // Fall back to alphabetical order.
            SortAlphabetically();
        }

        [RelayCommand]
        public void SortAlphabetically()
        {
            var sorted = RunSync(() => shopService.SortAlphabeticallyAsync());
            ReplaceShops(sorted);
        }

        [RelayCommand]
        public void LoadShops()
        {
            var shops = RunSync(() => shopService.GetAllAvailableShopsAsync());
            ReplaceShops(shops);
        }

        private void ReplaceShops(IEnumerable<Shop> shops)
        {
            Shops.Clear();
            foreach (var shop in shops)
            {
                Shops.Add(shop);
            }
        }

        private static T RunSync<T>(Func<Task<T>> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();

        private static void RunSync(Func<Task> taskFactory) =>
            Task.Run(taskFactory).GetAwaiter().GetResult();

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
