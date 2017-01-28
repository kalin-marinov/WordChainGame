using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data.Models;

namespace WordChainGame.Data
{
    public class TopicsManager
    {
        private IMongoDatabase database;

        public TopicsManager(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IReadOnlyCollection<Topic>> GetTopics()
            => await database.GetCollection<Topic>("Topics").AsQueryable().ToListAsync();

        public Task AddTopic(string name)
        {
            var topic = new Topic { Name = name, Words = new List<Word>() };
            var topics = database.GetCollection<Topic>("Topics");
            return topics.InsertOneAsync(topic);
        }

        public async Task<IEnumerable<Word>> GetWords(string topic)
        {
            var topics = database.GetCollection<Topic>("Topics");
            var topicData = await topics.Find(t => t.Name == topic).SingleAsync();

            return topicData.Words;
        }

        public async Task AddWord(string topic, string newWord, string author)
        {
            var topics = database.GetCollection<Topic>("Topics");

            // This query is a bit more efficient than: topics.AsQueryable().Where(t => t.Name == topic).Select(t => t.Words.LastOrDefault());
            var lastWordQuery = topics
                .Find(Builders<Topic>.Filter.Eq(t => t.Name, topic))
                .Project(Builders<Topic>.Projection.Expression(t => t.Words.LastOrDefault()));

            var lastWord = lastWordQuery.FirstOrDefault()?.Value;
            if (lastWord != null && lastWord.Last() != newWord.First())
                throw new ArgumentException("the new word must start with the last letter of the previo word");

            var wordItem = new Word { Value = newWord, Author = author };

            var updateAction = Builders<Topic>.Update.AddToSet(t => t.Words, wordItem);
            await topics.UpdateOneAsync(t => t.Name == topic, updateAction);
        }
    }
}
