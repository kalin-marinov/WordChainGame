using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Mongo.Models;
using Xunit;

namespace WordChainGame.Data.Tests
{
    public class TopicStoreTests
    {
        IMongoClient client;
        IMongoDatabase db;
        TopicStore manager;

        public TopicStoreTests()
        {
            client = new MongoClient("mongodb://admin:admin@ds135029.mlab.com:35029/wordgame");
            db = client.GetDatabase("wordgame");
            manager = new TopicStore(db);

            db.DropCollection("Topics");
            db.CreateCollection("Topics");
        }

        [Fact]
        public async Task CanManageTopics()
        {
            var name = Guid.NewGuid().ToString();
            await manager.AddTopicAsync(new MongoTopic { Name = name });

            var allTopicsNames = (await manager.GetAllTopicsAsync()).Select(x=> x.Name);
            Assert.Contains(name, allTopicsNames);
        }

        [Fact]
        public async Task CanManageTopicWords()
        {
            var name = Guid.NewGuid().ToString();
            await AddFewWords(name);

            var topicWords = await manager.GetWords(name);

            Assert.Equal("test1", topicWords.First().Value);
            Assert.Equal("tester", topicWords.First().Author);
            Assert.Equal("4test5", topicWords.Last().Value);
            Assert.Equal("tester", topicWords.Last().Author);
        }

        [Fact]
        public async Task CanRemoveTopicWords()
        {
            var name = Guid.NewGuid().ToString();
            await AddFewWords(name);

            await manager.DeleteWordAsync(name, "3test4");

            var topicWords = await manager.GetWords(name);

            Assert.Equal(4, topicWords.Count());
        }

        [Fact]
        public async Task CanPaginateTopicWords()
        {
            var name = Guid.NewGuid().ToString();
            await AddFewWords(name);

            var topicWords = await manager.GetWords(name, skip:2, take: 2);

            Assert.Equal("2test3", topicWords.First().Value);
            Assert.Equal(2, topicWords.Count());
        }

        [Fact]
        public async Task CanPaginateTopics()
        {
            await manager.AddTopicAsync(new MongoTopic { Name = "t1", Words = new Word[0] });
            await manager.AddTopicAsync(new MongoTopic { Name = "t2", Words = new Word[0] });
            await manager.AddTopicAsync(new MongoTopic { Name = "t3", Words = new Word[0] });
            await manager.AddTopicAsync(new MongoTopic { Name = "t4", Words = new Word[0] });
            await manager.AddTopicAsync(new MongoTopic { Name = "t5", Words = new Word[0] });

            var topics = await manager.GetTopicNames(skip: 2, take: 2, sortField: null);

            Assert.Equal("t3", topics.First());
            Assert.Equal(2, topics.Count());
        }

        private async Task AddFewWords(string name)
        {
            await manager.AddTopicAsync(new MongoTopic { Name = name, Words = new Word[0] });

            await manager.AddWordAsync(name, new Word { Value = "test1", Author = "tester" });
            await manager.AddWordAsync(name, new Word { Value = "1test2", Author = "tester" });
            await manager.AddWordAsync(name, new Word { Value = "2test3", Author = "tester" });
            await manager.AddWordAsync(name, new Word { Value = "3test4", Author = "tester" });
            await manager.AddWordAsync(name, new Word { Value = "4test5", Author = "tester" });
        }
    }
}
