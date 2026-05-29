using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ShopService(IShopRepository shopRepository) : IShopService
{
    private const string EmptyNameErrorMessage = "The shop name field must not be empty.";
    private const string EmptyTypeErrorMessage = "The shop type field must not be empty.";
    private const string DuplicateNameErrorMessage = "A shop with this name already exists in the system.";

    public async Task<IEnumerable<Shop>> GetAllAvailableShopsAsync()
    {
        return await shopRepository.GetAsync();
    }

    public async Task<Shop?> GetShopByIdAsync(int shopId)
    {
        return await shopRepository.GetByIdAsync(shopId);
    }

    public async Task AddShopAsync(Shop shopToAdd)
    {
        if (string.IsNullOrWhiteSpace(shopToAdd.Name))
        {
            throw new ArgumentException(EmptyNameErrorMessage, nameof(shopToAdd));
        }

        if (string.IsNullOrWhiteSpace(shopToAdd.Type))
        {
            throw new ArgumentException(EmptyTypeErrorMessage, nameof(shopToAdd));
        }

        bool doesNameAlreadyExist = await CheckIfNameExistsAsync(shopToAdd.Name);
        if (doesNameAlreadyExist)
        {
            throw new InvalidOperationException(DuplicateNameErrorMessage);
        }

        await shopRepository.AddAsync(shopToAdd);
    }

    public async Task UpdateShopAsync(Shop shopToUpdate)
    {
        if (string.IsNullOrWhiteSpace(shopToUpdate.Name))
        {
            throw new ArgumentException(EmptyNameErrorMessage, nameof(shopToUpdate));
        }

        if (string.IsNullOrWhiteSpace(shopToUpdate.Type))
        {
            throw new ArgumentException(EmptyTypeErrorMessage, nameof(shopToUpdate));
        }

        bool isDuplicateName = await CheckIfNameIsDuplicateForOtherShopAsync(shopToUpdate.Id, shopToUpdate.Name);
        if (isDuplicateName)
        {
            throw new InvalidOperationException(DuplicateNameErrorMessage);
        }

        await shopRepository.UpdateAsync(shopToUpdate);
    }

    public async Task DeleteShopAsync(int shopId)
    {
        await shopRepository.DeleteAsync(shopId);
    }

    public async Task<IEnumerable<Shop>> SortAlphabeticallyAsync()
    {
        IEnumerable<Shop> allShops = await GetAllAvailableShopsAsync();
        List<Shop> sortedList = new List<Shop>(allShops);
        sortedList.Sort(new ShopNameComparer());
        return sortedList;
    }

    public async Task<IEnumerable<Shop>> SearchByNameAsync(string searchText)
    {
        IEnumerable<Shop> allShops = await GetAllAvailableShopsAsync();
        List<Shop> matchingShops = new List<Shop>();

        foreach (Shop shop in allShops)
        {
            if (shop.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                matchingShops.Add(shop);
            }
        }

        return matchingShops;
    }

    private async Task<bool> CheckIfNameExistsAsync(string nameToFind)
    {
        foreach (Shop existingShop in await shopRepository.GetAsync())
        {
            if (string.Equals(existingShop.Name, nameToFind, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<bool> CheckIfNameIsDuplicateForOtherShopAsync(int currentShopId, string nameToVerify)
    {
        foreach (Shop otherShop in await shopRepository.GetAsync())
        {
            if (otherShop.Id != currentShopId && string.Equals(otherShop.Name, nameToVerify, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private class ShopNameComparer : IComparer<Shop>
    {
        public int Compare(Shop? firstShop, Shop? secondShop)
        {
            if (firstShop == null || secondShop == null)
            {
                return 0;
            }

            return string.Compare(firstShop.Name, secondShop.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
