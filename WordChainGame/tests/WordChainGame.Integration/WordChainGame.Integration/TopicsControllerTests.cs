using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WordChainGame.Integration
{
    public class TopicsControllerTests : ControllerTestBase
    {
        [Fact]
        public async Task CanCreateTopic()
        {
            await Login();

            var postBody = new Dictionary<string, string>
             {
                 { "name", "test" },
             };

            var response = await client.PostAsync("/api/v1/topics", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateTopicRejectsDuplicateNames()
        {
            await CanCreateTopic();

            var postBody = new Dictionary<string, string>
             {
                 { "name", "test" },
             };

            var response = await client.PostAsync("/api/v1/topics", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("Topic already exists", responseString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async Task GetTopicsShouldReturnCorrectData()
        {
            await CanCreateTopic();

            var response = await client.GetAsync($"/api/v1/topics?skip=0&take=10&sortBy=name");
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("test", responseString);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTopicsRejectsBadFilters()
        {
            await CanCreateTopic();

            var response = await client.GetAsync($"/api/v1/topics");
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetTopicsRequiresAuthentication()
        {
            var response = await client.GetAsync($"/api/v1/topics");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateTopicRequiresAuthorization()
        {
            var postBody = new Dictionary<string, string>
             {
                 { "name", "test" },
             };

            var response = await client.PostAsync("/api/v1/topics", new FormUrlEncodedContent(postBody));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
