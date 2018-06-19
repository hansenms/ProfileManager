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

namespace ProfileManager.Tests
{
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
        [InlineData("/api/profile/1")]
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

        [Fact]
        public async Task Get_GetByIdReturnsNotFoundWhenProfileDoesntExit()
        {
            // Arrange
            var client = _factory.CreateClient();
            string url = "/api/profile/1000";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post_CreateReturns201()
        {
            // Arrange
            var client = _factory.CreateClient();
            string url = "/api/profile";

            var requestData = new { firtName = "TestFirstName",
                                    lastName = "TestLastName",
                                    department = "TestDepartment",
                                    photo = "TestPhoto.jpg" };


            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }


        [Fact]
        public async Task Update_Returns204WhenProfileExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            string url = "/api/profile/1";

            var requestData = new { firtName = "TestFirstName",
                                    lastName = "TestLastName",
                                    department = "TestDepartment",
                                    photo = "TestPhoto.jpg" };

            // Act
            var response = await client.PutAsJsonAsync(url, requestData);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update_Returns404WhenProfileDoesntExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            string url = "/api/profile/1000";

            var requestData = new { firtName = "TestFirstName",
                                    lastName = "TestLastName",
                                    department = "TestDepartment",
                                    photo = "TestPhoto.jpg" };


            // Act
            var response = await client.PutAsJsonAsync(url, requestData);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Returns204WhenProfileExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            string url = "/api/profile/1";


            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Returns404WhenProfileDoesntExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            string url = "/api/profile/1000";

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


    }
}
