namespace api_app.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using api_app.Models;

    public interface IItemService
    {
        Task<Item> GetItemAsync(string id);
        Task<IEnumerable<Item>> GetAsync(string query);
        Task AddAsync(Item item);
        Task UpdateAsync(Item item);
    }
}