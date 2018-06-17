using System;
using Xunit;
using ProfileManager.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

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

    public class ProfileAPITest
        : IClassFixture<CustomWebApplicationFactory<ProfileManager.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<ProfileManager.Startup> _factory;

        public ProfileAPITest(CustomWebApplicationFactory<ProfileManager.Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/profile")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }

    }
}
