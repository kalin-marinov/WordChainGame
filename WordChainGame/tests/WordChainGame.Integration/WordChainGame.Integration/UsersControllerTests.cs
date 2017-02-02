using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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


            var response = await client.PostAsync("/api/users", new FormUrlEncodedContent(postBody));
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

            var response = await client.PostAsync("/api/users", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Created", responseString);
            response.EnsureSuccessStatusCode();
        }

    }
}
