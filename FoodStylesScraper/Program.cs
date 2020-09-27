using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FoodStylesScraper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, builder) =>
                {
                    var configuration = hostingContext.Configuration.GetSection("Logging");
                    builder.AddFile(configuration);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
