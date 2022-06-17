using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.Data;
using PostService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Controllers
{
    [EnableCors("AllowAll")]
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICosmosDBService _cosmosDbService;
        public CategoryController(ICosmosDBService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
        }
        // GET api/items
        [HttpGet]
        public async Task<IActionResult> List()
        {
            //reid cache 
            return Ok(await _cosmosDbService.GetMultipleAsync("SELECT * FROM c"));
        }
        // GET api/items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _cosmosDbService.GetAsync(id));
        }
    }
}
