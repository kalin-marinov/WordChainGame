using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WordChainGame.Integration
{
    public class UsersControllerTests : ControllerTestBase
    {
        [Fact]
        public async Task RejectsInvalidUserData()
        {
            var postBody = new Dictionary<string, string>
             {
                 { "username", "test" },
                 { "password", "Test123!" },
                 { "email", "invalid" }
             };


            var response = await client.PostAsync("/api/v1/users", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Email", responseString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CanCreateValidUser()
        {
            var postBody = new Dictionary<string, string>
             {
                 { "username", "tester" },
                 { "password", "Test123!" },
                 { "fullname", "tester" },
                 { "email", "test@test.test" }
             };

            var response = await client.PostAsync("/api/v1/users", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Created", responseString);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DelteUserRemovesAllData()
        {
            await Login();

            var request = new HttpRequestMessage
            {
                Content = new StringContent($"password=Test123!", Encoding.UTF8, "application/x-www-form-urlencoded"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("http://localhost/api/v1/users/test")
            };


            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}
