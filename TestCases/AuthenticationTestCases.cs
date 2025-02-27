using System.Net.Http.Headers;
using System.Net;
//using Microsoft.AspNetCore.TestHost;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;


namespace TestCases
{
    public class AuthenticationTestCases : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
       // private readonly HttpClient _client;
        public AuthenticationTestCases(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        private HttpClient CreateClient(string role)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).AddJwtBearer(options =>
                    {
                        options.Authority = "https://fake-authority.com";
                        options.Audience = "fake-audience";
                    });
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "fake-token");
            client.DefaultRequestHeaders.Add("Role", role);

            return client;
        }

        [Fact]
        public async Task Admin_Can_Access_AllUsers()
        {
            var client = CreateClient("Admin");
            var response = await client.GetAsync("/api/user/allTasks");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
