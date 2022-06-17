using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //Microsoft.EntityFrameworkCore.Storage.IDatabase cache = lazyConnection.Value.GetDatabase();
        //private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        //{
        //    string cacheConnection = configs.RedisCache;
        //    return ConnectionMultiplexer.Connect(cacheConnection);
        //});

        //public static ConnectionMultiplexer Connection
        //{
        //    get
        //    {
        //        return lazyConnection.Value;
        //    }
        //}
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var buittconfiguration = config.Build();
                    string KvURL = buittconfiguration["KeyVaultconfig:KVUrl"];
                    string tenantId = buittconfiguration["KeyVaultconfig:TenantId"];
                    string clientId = buittconfiguration["KeyVaultconfig:ClientId"];
                    string clientSecret = buittconfiguration["KeyVaultconfig:ClientSecretId"];
                    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                    var client = new SecretClient(new Uri(KvURL), credential);
                    config.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());
                    KeyVaultSecret Option = client.GetSecret("UserDB");
                    KeyVaultSecret key = client.GetSecret("servicebusstring");
                });
    }
}
