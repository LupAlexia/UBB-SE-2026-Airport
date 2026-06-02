using AirportLib.Domain.User;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.DutyFree
{
    public class ShopListViewModel
    {
        public List<Shop> Shops { get; set; } = new();
        public string SearchQuery { get; set; } = string.Empty;
        public DutyFreeModuleRole UserRole { get; set; }
        public bool IsManager => UserRole == DutyFreeModuleRole.Manager;
        public ShopFormModel AddShopForm { get; set; } = new();
        public bool ShowAddShopForm { get; set; }
    }

    public class ShopFormModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int ManagerId { get; set; }
    }
}

