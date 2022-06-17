using Microsoft.Azure.Cosmos;
using PostService.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Repository
{
    public class CosmosDBService: ICosmosDBService
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;
        public CosmosDBService(CosmosClient cosmosDbClient,string databaseName,string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddAsync(CategoryDetails item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.id));
        }
        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<CategoryDetails>(id, new PartitionKey(id));
        }
        public async Task<CategoryDetails> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<CategoryDetails>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }
        public async Task<IEnumerable<CategoryDetails>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<CategoryDetails>(new QueryDefinition(queryString));
            var results = new List<CategoryDetails>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task UpdateAsync(string id, CategoryDetails item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }
    }
}
