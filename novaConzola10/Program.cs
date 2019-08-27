using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace novaConzola10
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient("ApiClient", c =>
                {
                    c.BaseAddress = new Uri("http://apitest");
                });
                services.AddTransient<IMyService, MyService>();
            }).UseConsoleLifetime();


            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var myService = services.GetRequiredService<IMyService>();
                    var data = await myService.GetData();

                    Console.WriteLine(data);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred.");
                }
            }

            return 0;

        }

        public interface IMyService
        {
            Task<string> GetData();
        }

        public class MyService : IMyService
        {
            private readonly IHttpClientFactory _clientFactory;

            public MyService(IHttpClientFactory clientFactory)
            {
                _clientFactory = clientFactory;
            }

            public async Task<string> GetData()
            {
                var clientF = _clientFactory;
                var http = clientF.CreateClient("ApiClient");
                var client = new MojKlijent.Klijent(http.BaseAddress.ToString(), clientF);
                var response = await client.GetTeamDetailsAsync(11);
                return response.ToString();

            }
        }
    }
}
