using PostService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Repository
{
    public interface ICosmosDBService
    {
        Task<IEnumerable<CategoryDetails>> GetMultipleAsync(string query);
        Task<CategoryDetails> GetAsync(string id);
        Task AddAsync(CategoryDetails item);
        Task UpdateAsync(string id, CategoryDetails item);
        Task DeleteAsync(string id);
    }
}
