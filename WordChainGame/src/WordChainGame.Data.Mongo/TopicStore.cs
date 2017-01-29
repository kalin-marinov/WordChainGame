using EnsureThat;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WordChainGame.Data.Exceptions;
using WordChainGame.Data.Models;
using WordChainGame.Data.Mongo.Models;

namespace WordChainGame.Data
{
    public class TopicStore : ITopicStore<MongoTopic>
    {
        private IMongoDatabase database;

        private IMongoCollection<MongoTopic> topics => database.GetCollection<MongoTopic>("Topics");

        public TopicStore(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IReadOnlyCollection<MongoTopic>> GetAllTopicsAsync()
            => await database.GetCollection<MongoTopic>("Topics").AsQueryable().ToListAsync();

        public async Task<IReadOnlyCollection<string>> GetAllTopicNames()
            => await topics.Find(_ => true)
                 .Project(Builders<MongoTopic>.Projection.Expression(t => t.Name))
                 .ToListAsync();


        public async Task<IReadOnlyCollection<string>> GetTopicNames(int skip, int take, Expression<Func<MongoTopic, object>> sortField)
        {
            var query = topics.Find(_ => true)
                 .Skip(skip)
                 .Limit(take)
                 .Project(Builders<MongoTopic>.Projection.Expression(t => t.Name));

            if (sortField != null)
                query = query.SortBy(sortField);

            return await query.ToListAsync();
        }

        public Task AddTopicAsync(MongoTopic topic)
            => topics.InsertOneAsync(topic);


        public Task<bool> TopicExistsAsync(string topic)
          => topics.Find(t => t.Name == topic).AnyAsync();

        public async Task DeleteWordAsync(string topic, string word)
        {
            var updateAction = Builders<MongoTopic>.Update.PullFilter(t => t.Words, w => w.Value == word);
            var result = await topics.UpdateOneAsync(t => t.Name == topic, updateAction);

            Ensure.That(result.MatchedCount).IsNot(0).WithException(_ => new TopicNotFoundException(topic));
            Ensure.That(result.ModifiedCount).IsNot(0).WithException(_ => new WordNotFoundException(word));
        }

        public async Task<IEnumerable<Word>> GetWords(string topic)
        {
            var topicData = await topics.Find(t => t.Name == topic).SingleOrDefaultAsync();
            Ensure.That(topicData).IsNotNull().WithException(_ => new TopicNotFoundException(topic));

            return topicData.Words;
        }

        public async Task<IEnumerable<Word>> GetWords(string topic, int skip, int take)
        {
            var topicData = topics.Find(t => t.Name == topic)
                .Project(t => t.Words.Skip(skip).Take(take));

            var words = await topicData.SingleOrDefaultAsync();
            Ensure.That(words).IsNotNull().WithException(_ => new TopicNotFoundException(topic));

            return words;
        }

        public async Task AddWordAsync(string topic, Word word)
        {
            var updateAction = Builders<MongoTopic>.Update.Push(t => t.Words, word);
            var result = await topics.UpdateOneAsync(t => t.Name == topic, updateAction);

            Ensure.That(result.MatchedCount).IsNot(0).WithException(_ => new TopicNotFoundException(topic));
        }

        public async Task<Word> GetLastWord(string topic)
        {
            var lastWordQuery = topics
               .Find(Builders<MongoTopic>.Filter.Eq(t => t.Name, topic))
               .Project(Builders<MongoTopic>.Projection.Expression(t => t.Words.LastOrDefault()));

            return await lastWordQuery.FirstOrDefaultAsync();
        }

        public Task<bool> WordExists(string topic, string word, string author)
           => topics.Find(t => t.Name == topic && t.Words.Any(w => w.Value == word && w.Author == author)).AnyAsync();
    }
}
