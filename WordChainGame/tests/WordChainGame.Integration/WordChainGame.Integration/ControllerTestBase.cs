using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WordChainGame.Integration
{
    public class ControllerTestBase
    {
        protected HttpClient client;
        protected TestServer server;

        public ControllerTestBase()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            client = server.CreateClient();
            CleanData();
        }

        public void CleanData()
        {
            var options = new ConfigurationOptions { AllowAdmin = true };
            options.EndPoints.Add("localhost:6379");

            var connection = ConnectionMultiplexer.Connect(options);
            connection.GetServer("localhost:6379").FlushAllDatabases();

            try
            {
                var mongo = new MongoClient("mongodb://admin:admin@ds135029.mlab.com:35029/wordgame");
                mongo.GetDatabase("wordgame").DropCollection("Topics");
                mongo.GetDatabase("wordgame").DropCollection("Reports");
                mongo.GetDatabase("wordgame").CreateCollection("Topics");
                mongo.GetDatabase("wordgame").CreateCollection("Reports");
            }
            catch { }
        }


        public async Task<HttpResponseMessage> Login()
        {
            try { await CreateUser(); }
            catch { }

            var postBody = new Dictionary<string, string>
             {
                 { "username", "test" },
                 { "password", "Test123!" }
             };

            var response = await client.PostAsync("/api/token", new FormUrlEncodedContent(postBody));
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
            var token = responseData["access_token"];

            if (client.DefaultRequestHeaders.Authorization == null)
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> CreateUser()
        {
            var postBody = new Dictionary<string, string>
             {
                 { "username", "test" },
                 { "password", "Test123!" },
                 { "fullname", "tester" },
                 { "email", "test@test.test" }
             };

            var response = await client.PostAsync("/api/users", new FormUrlEncodedContent(postBody));
            return response;
        }
    }
}
