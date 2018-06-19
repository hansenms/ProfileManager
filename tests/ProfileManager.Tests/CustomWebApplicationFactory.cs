using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProfileManager.Data;

namespace ProfileManager.Tests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<ProfileManager.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                // Add a database context (ProfileDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<ProfileContext>(options =>
                    {
                        options.UseInMemoryDatabase("ProfileDbForTesting");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ProfileContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    ProfileDbInitializer.Initialize(context);
                }
            });
        }
    }
}