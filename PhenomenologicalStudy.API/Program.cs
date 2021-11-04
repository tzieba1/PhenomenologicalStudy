using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhenomenologicalStudy.API.Configuration;
using PhenomenologicalStudy.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      
      var host = CreateHostBuilder(args).Build();
      var configuration = host.Services.GetService<IConfiguration>(); // Retrieve info from configuration to initialize app secrets
      var hosting = host.Services.GetService<IWebHostEnvironment>();  // Scoped service dependency injection for seeding DB with DbInitializer

      if (hosting.IsDevelopment()) // Initialize secrets from locally encrypted secrets.json file
      {
        var dbInitConfig = configuration.GetSection("DbInitializerConfig").Get<DbInitializerConfiguration>();
        DbInitializer.Configuration = dbInitConfig;
      }
      else // When not using development hosting, Azure configuration application settings are retrieved following naming convention 'Secrets__Key' - where 'Secrets' is like 'secrets.js' property
      {
        var dbInitConfig = configuration.GetSection("DbInitializerConfig").Get<DbInitializerConfiguration>();
        DbInitializer.Configuration = dbInitConfig;
      }

      // Seed users and roles with a scoped service
      using (IServiceScope scope = host.Services.CreateScope())
      {
        DbInitializer.SeedUsersAndRoles(scope.ServiceProvider).Wait();
      }
      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });

    //public static IHostBuilder CreateHostBuilder(string[] args) =>
    //    Host.CreateDefaultBuilder(args)
    //        .ConfigureAppConfiguration((context, config) =>
    //        {
    //          var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
    //          config.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
    //        })
    //        .ConfigureWebHostDefaults(webBuilder =>
    //        {
    //          webBuilder.UseStartup<Startup>();
    //        });
  }
}
