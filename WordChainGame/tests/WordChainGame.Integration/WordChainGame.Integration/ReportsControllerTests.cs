using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WordChainGame.Integration
{
    public class ReportsControllerTests : ControllerTestBase
    {
        const string TestTopic = "topic1";
        const string TestWord = "testWord";
        const string ValidUser = "test";

        private async Task<HttpResponseMessage> CreateTestTopic()
        {
            await Login();
            var postBody = new Dictionary<string, string> { { "name", TestTopic } };
            await client.PostAsync("/api/v1/topics", new FormUrlEncodedContent(postBody));

            postBody = new Dictionary<string, string> { { "Word", TestWord } };
            var result = await client.PostAsync($"/api/v1/topics/{TestTopic}/words", new FormUrlEncodedContent(postBody));

            return result;
        }

        [Fact]
        public async Task CanCreateReport()
        {
            await CreateTestTopic();

            var postBody = new Dictionary<string, string>
             {
                 { "reporter", "test" },
                 { "word", TestWord },
                 { "topic", TestTopic },
                 { "wordauthor", ValidUser },
             };

            var response = await client.PostAsync("/api/v1/reports", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }


        [Fact]
        public async Task CreateReportRejectsInvalidAuthor()
        {
            await CreateTestTopic();

            var postBody = new Dictionary<string, string>
             {
                 { "reporter", "test" },
                 { "word", TestWord },
                 { "topic", TestTopic },
                 { "wordauthor", "test123" },
             };

            var response = await client.PostAsync("/api/v1/reports", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("No word", responseString);
            Assert.Contains("by test123", responseString);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateReportRejectsInvalidTopic()
        {
            await CreateTestTopic();

            var postBody = new Dictionary<string, string>
             {
                 { "reporter", "test" },
                 { "word", TestWord },
                 { "topic", "invalid" },
                 { "wordauthor", ValidUser },
             };

            var response = await client.PostAsync("/api/v1/reports", new FormUrlEncodedContent(postBody));
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("Topic", responseString);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateReportRejectsNonAuthorized()
        {
            var postBody = new Dictionary<string, string>
             {
                 { "reporter", "testReporter" },
                 { "word", TestWord },
                 { "topic", "invalid" },
                 { "wordauthor", ValidUser },
             };

            var response = await client.PostAsync("/api/v1/reports", new FormUrlEncodedContent(postBody));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task CanGetReportsTopic()
        {
            await CreateTestTopic();

            //Create Report
            var postBody = new Dictionary<string, string>
            {
                { "reporter", "test" },
                 { "word", TestWord },
                 { "topic", TestTopic },
                 { "wordauthor", ValidUser},
             };

            await client.PostAsync("/api/v1/reports", new FormUrlEncodedContent(postBody));

            // Get All Reports
            var response = await client.GetAsync("/api/v1/reports");
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("test", responseString);
            Assert.Contains(TestWord, responseString);
            Assert.Contains(TestTopic, responseString);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
