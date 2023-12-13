using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PFWS.API;
using PFWS.DataAccessLayer.Data;
using PFWS.DataAccessLayer.Models;

namespace PFWS.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<ApiAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<WalletContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            File.Copy("wallet_test.db", "wallet_test_copy.db", overwrite: true);

            services.AddDbContext<WalletContext>(options =>
            {
                options.UseSqlite("Data Source=wallet_test_copy.db");
            });

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<WalletContext>();
                var userManager = scopedServices.GetRequiredService<UserManager<User>>();

                db.Database.OpenConnection();
                db.Database.EnsureCreated();

                // SeedTestData(db, userManager).Wait();
            }
        });
    }

    // private async Task SeedTestData(WalletContext db, UserManager<User> userManager)
    // {
    //     await DbInitializer.Initialize(db, userManager);
    // }
}
