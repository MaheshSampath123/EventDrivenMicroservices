using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PostService.Data;
using PostService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddControllers();
            services.AddDbContext<PostDBContext>(op => op.UseSqlServer("Data Source=tcp:generacsqlserver.database.windows.net;Initial Catalog=PostService;User ID=sqlAdmin;Password=generac@D360"));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddSingleton<ICosmosDBService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration["RedisConnectionString"];
                // options.Configuration = Configuration.GetValue<string>("RedisConnectionString");
                options.InstanceName = "master";
            });
        }

        private static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];
            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
            var cosmosDbService = new CosmosDBService(client, databaseName, containerName);
            return cosmosDbService;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowOrigin");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
