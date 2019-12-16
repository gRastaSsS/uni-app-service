namespace api_app.Services 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using api_app.Models;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Extensions.Configuration;

    public class ItemService : IItemService
    {
        private Container _container;

        public ItemService(Container container)
        {
            this._container = container;
        }

        public async Task AddAsync(Item item) 
        {
            await this._container.CreateItemAsync<Item>(item);
        }

        public async Task UpdateAsync(Item item)
        {
            await this._container.UpsertItemAsync<Item>(item);
        }

        public async Task<IEnumerable<Item>> GetAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
            List<Item> results = new List<Item>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await this._container.ReadItemAsync<Item>(id, PartitionKey.None);
        }
    }
}