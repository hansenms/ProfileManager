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
using System.Net;
using Newtonsoft.Json;
using System.Text;
using Xunit.Abstractions;

namespace ProfileManager.Tests
{
    public class ProfileAppTest
        : IClassFixture<CustomWebApplicationFactory<ProfileManager.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<ProfileManager.Startup> _factory;

        public ProfileAppTest(CustomWebApplicationFactory<ProfileManager.Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }


    }
}