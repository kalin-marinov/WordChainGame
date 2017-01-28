using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace WordChainGame.Data.Tests
{
    public class TestClass
    {
        IMongoClient client;
        IMongoDatabase db;
        TopicsManager manager;


        public TestClass()
        {
            client = new MongoClient("mongodb://admin:admin@ds135029.mlab.com:35029/wordgame");
            db = client.GetDatabase("wordgame");
            manager = new TopicsManager(db);

            db.DropCollection("Topics");
            db.CreateCollection("Topics");
        }

        [Fact]
        public async Task CanManageTopics()
        {
            var name = Guid.NewGuid().ToString();
            await manager.AddTopic(name);

            var allTopicsNames = (await manager.GetTopics()).Select(x=> x.Name);
            Assert.Contains(name, allTopicsNames);
        }

        [Fact]
        public async Task CanManageTopicWords()
        {
            var name = Guid.NewGuid().ToString();
            await manager.AddTopic(name);

            await manager.AddWord(name, "test1", "tester");
            await manager.AddWord(name, "1test2", "tester");
            await manager.AddWord(name, "2test3", "tester");
            await manager.AddWord(name, "3test4", "tester");

            var topicWords = await manager.GetWords(name);

            Assert.Equal("test1", topicWords.First().Value);
            Assert.Equal("tester", topicWords.First().Author);
            Assert.Equal("3test4", topicWords.Last().Value);
            Assert.Equal("tester", topicWords.Last().Author);
        }
    }
}
