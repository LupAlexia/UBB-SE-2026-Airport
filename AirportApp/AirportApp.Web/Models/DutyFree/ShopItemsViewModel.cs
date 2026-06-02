using AirportLib.Domain.User;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.DutyFree
{
    public class ShopItemsViewModel
    {
        public Shop Shop { get; set; } = null!;
        public List<ShopItem> Items { get; set; } = new();
        public string SearchQuery { get; set; } = string.Empty;
        public string SortOrder { get; set; } = "default";
        public DutyFreeModuleRole UserRole { get; set; }
        public bool IsManager => UserRole == DutyFreeModuleRole.Manager;
        public ShopItemFormModel AddItemForm { get; set; } = new();
        public bool ShowAddItemForm { get; set; }
    }

    public class ShopItemFormModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Quantity { get; set; }
        public int ShopId { get; set; }
    }
}

