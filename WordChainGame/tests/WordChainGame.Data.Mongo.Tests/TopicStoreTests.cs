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
        TopicStore store;

        public TopicStoreTests()
        {
            client = new MongoClient("mongodb://admin:admin@ds135029.mlab.com:35029/wordgame");
            db = client.GetDatabase("wordgame");
            store = new TopicStore(db);

            db.DropCollection("Topics");
            db.CreateCollection("Topics");
        }

        [Fact]
        public async Task CanManageTopics()
        {
            var name = Guid.NewGuid().ToString();
            await store.AddTopicAsync(new MongoTopic { Name = name });

            var allTopicsNames = (await store.GetAllTopicsAsync()).Select(x=> x.Name);
            Assert.Contains(name, allTopicsNames);
        }

        [Fact]
        public async Task CanManageTopicWords()
        {
            var name = Guid.NewGuid().ToString();
            await AddFewWords(name);

            var topicWords = await store.GetWords(name);

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

            await store.DeleteWordAsync(name, "3test4");

            var topicWords = await store.GetWords(name);

            Assert.Equal(4, topicWords.Count());
        }

        [Fact]
        public async Task CanPaginateTopicWords()
        {
            var name = Guid.NewGuid().ToString();
            await AddFewWords(name);

            var topicWords = await store.GetWords(name, skip:2, take: 2);

            Assert.Equal("2test3", topicWords.First().Value);
            Assert.Equal(2, topicWords.Count());
        }


        [Fact]
        public async Task CanPaginateTopicDescription()
        {
            await store.AddTopicAsync(new MongoTopic { Name = "t1", Words = new Word[0] });
            await store.AddTopicAsync(new MongoTopic { Name = "t2", Words = new Word[0] });
            await store.AddTopicAsync(new MongoTopic { Name = "t3", Words = new Word[0] });
            await store.AddTopicAsync(new MongoTopic { Name = "t4", Words = new Word[0] });
            await store.AddTopicAsync(new MongoTopic { Name = "t5", Words = new Word[0] });

            var topics = await store.GetTopicDescriptions(2, 2);

            Assert.Equal("t3", topics.First().Name);
            Assert.Equal(2, topics.Count());
        }

        private async Task AddFewWords(string name)
        {
            await store.AddTopicAsync(new MongoTopic { Name = name, Words = new Word[0] });

            await store.AddWordAsync(name, new Word { Value = "test1", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "1test2", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "2test3", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "3test4", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "4test5", Author = "tester" });
        }
    }
}
