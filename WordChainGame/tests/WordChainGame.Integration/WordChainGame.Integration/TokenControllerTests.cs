using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WordChainGame.Integration
{
    public class TokenControllerTests : ControllerTestBase
    {

        [Fact]
        public async Task CanLoginWithValidCredentials()
        {
            var response = await base.Login();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("access_token", responseString);
            Assert.Contains("expires", responseString);
            response.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task RejectsInValidCredentials()
        {
            var postBody = new Dictionary<string, string>
             {
                 { "username", "test" },
                 { "password", "Test123!" }
             };

            var response = await client.PostAsync("/api/token", new FormUrlEncodedContent(postBody));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
