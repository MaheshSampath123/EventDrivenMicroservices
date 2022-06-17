using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace PostService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureWebJobs((context,builder)=> {
                    builder.AddServiceBus(options => options.MaxConcurrentCalls = 1);
                    var hostBuilder = new HostBuilder();
                    hostBuilder.Build().RunAsync();
                });
    }
}
