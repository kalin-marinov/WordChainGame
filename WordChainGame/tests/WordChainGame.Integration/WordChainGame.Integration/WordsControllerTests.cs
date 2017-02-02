using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WordChainGame.Integration
{
    public class WordsControllerTests : ControllerTestBase
    {
        const string TestTopic = "topic1";

        private async Task<HttpResponseMessage> CreateTestTopic()
        {
            await Login();
            var postBody = new Dictionary<string, string> { { "name", TestTopic } };
            var result = await client.PostAsync("/api/v1/topics", new FormUrlEncodedContent(postBody));
            return result;
        }

        [Fact]
        public async Task CanAddWordToValidTopic()
        {
            (await CreateTestTopic()).EnsureSuccessStatusCode();

            var postBody = new Dictionary<string, string> { { "Word", "testWord" } };
            var response = await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Fact]
        public async Task CanDeleteWord()
        {
            const string word = "testWord";
            (await CreateTestTopic()).EnsureSuccessStatusCode();

            var postBody = new Dictionary<string, string> { { "Word", word } };
            await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));

            var response  = await client.DeleteAsync($"/api/v1/topics/{TestTopic}/words/{word}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Fact]
        public async Task PostWordRejectsInvalidTopic()
        {
            await Login();

            var postBody = new Dictionary<string, string> { { "word", "testWord" } };
            var response = await client.PostAsync("/api/v1/topics/Invalid/words", new FormUrlEncodedContent(postBody));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddWordRejectsInvalidWords()
        {
            (await CreateTestTopic()).EnsureSuccessStatusCode();

            var postBody = new Dictionary<string, string> { { "Word", "testWord" } };
            var response = await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));

            postBody = new Dictionary<string, string> { { "Word", "wrongWord" } };
            response = await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("The new word must start with the last letter of the previous one", responseString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async Task AddWordRejectsNonAuthorized()
        {
            var postBody = new Dictionary<string, string> { { "Word", "testWord" } };
            var response = await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CanAddMultipleWords()
        {
            (await CreateTestTopic()).EnsureSuccessStatusCode();

            var postBody = new Dictionary<string, string> { { "Word", "testWord" } };
            var response = await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));

            postBody = new Dictionary<string, string> { { "Word", "daRightWord" } };
            response = await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}
