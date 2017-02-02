using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
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
            await store.AddTopicAsync(name, "somebody");

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

            var topicWords = await store.GetWordsAsync(name, skip:2, take: 2);

            Assert.Equal("2test3", topicWords.First().Value);
            Assert.Equal(2, topicWords.Count());
        }


        [Fact]
        public async Task CanPaginateTopicDescription()
        {
            await store.AddTopicAsync("t1", "pesho");
            await store.AddTopicAsync("t2", "pesho");
            await store.AddTopicAsync("t3", "pesho");
            await store.AddTopicAsync("t4", "pesho");
            await store.AddTopicAsync("t5", "pesho");

            var topics = await store.GetTopicsAsync(2, 2, "Name");

            Assert.Equal("t3", topics.First().Name);
            Assert.Equal(2, topics.Count());
        }

        [Fact]
        public async Task CanBlackListWord()
        {
            await store.AddTopicAsync("t1", "pesho");
            await store.AddToBlackListAsync("t1", "word1");
            var w1BlackList = await store.IsBlackListedAsync("t1", "word1");
            var w2shoBlackList = await store.IsBlackListedAsync("t1", "word2");

            Assert.True(w1BlackList);
            Assert.False(w2shoBlackList);
        }


        private async Task AddFewWords(string name)
        {
            await store.AddTopicAsync(name, "pesho");

            await store.AddWordAsync(name, new Word { Value = "test1", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "1test2", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "2test3", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "3test4", Author = "tester" });
            await store.AddWordAsync(name, new Word { Value = "4test5", Author = "tester" });
        }
    }
}
