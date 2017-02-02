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
using WordChainGame.Data.Topics.Models;

namespace WordChainGame.Data
{
    public class TopicStore : ITopicStore
    {
        private IMongoDatabase database;

        private IMongoCollection<MongoTopic> topics => database.GetCollection<MongoTopic>("Topics");

        public TopicStore(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IReadOnlyCollection<MongoTopic>> GetAllTopicsAsync()
            => await database.GetCollection<MongoTopic>("Topics").AsQueryable().ToListAsync();


        public async Task<IReadOnlyCollection<TopicDescription>> GetTopicsAsync(
            int skip, int take, string sortField)
        {
            var query = topics.Find(_ => true)
                 .Sort(Builders<MongoTopic>.Sort.Ascending(sortField))
                 .Skip(skip)
                 .Limit(take)
                 .Project(Builders<MongoTopic>.Projection.Expression(t =>
                      new TopicDescription
                      {
                          Name = t.Name,
                          Author = t.Author,
                          WordCount = t.WordCount
                      }));

            return await query.ToListAsync();
        }

        public Task AddTopicAsync(string topicName, string author)
        {
            var topic = new MongoTopic
            {
                Name = topicName,
                Author = author,
                Words = new Word[0],
                WordCount = 0,
                BlackList = new string[0]
            };

            return topics.InsertOneAsync(topic);
        }


        public Task<bool> TopicExistsAsync(string topic)
          => topics.Find(t => t.Name == topic).AnyAsync();

        public async Task DeleteWordAsync(string topic, string word)
        {
            var updateAction = Builders<MongoTopic>.Update.PullFilter(t => t.Words, w => w.Value == word);

            var updateResult = await topics.UpdateOneAsync(t => t.Name == topic, updateAction);
            Ensure.That(updateResult.MatchedCount).IsNot(0).WithException(_ => new TopicNotFoundException(topic));
            Ensure.That(updateResult.ModifiedCount).IsNot(0).WithException(_ => new WordNotFoundException(word, topic));

            var decrementAction = Builders<MongoTopic>.Update.Inc(t => t.WordCount, -updateResult.ModifiedCount);
            await topics.UpdateOneAsync(t => t.Name == topic, decrementAction);
        }

        public async Task<IEnumerable<Word>> GetWords(string topic)
        {
            var topicData = await topics.Find(t => t.Name == topic).SingleOrDefaultAsync();
            Ensure.That(topicData).IsNotNull().WithException(_ => new TopicNotFoundException(topic));

            return topicData.Words;
        }

        public async Task<IEnumerable<Word>> GetWordsAsync(string topic, int skip, int take)
        {
            var topicData = topics.Find(t => t.Name == topic)
                .Project(t => t.Words.Skip(skip).Take(take));

            var words = await topicData.SingleOrDefaultAsync();
            Ensure.That(words).IsNotNull().WithException(_ => new TopicNotFoundException(topic));

            return words;
        }

        public async Task AddWordAsync(string topic, Word word)
        {
            var addWordAction = Builders<MongoTopic>.Update.Push(t => t.Words, word);
            var incrementAction = Builders<MongoTopic>.Update.Inc(t => t.WordCount, 1);

            var addWordResult = await topics.UpdateOneAsync(t => t.Name == topic, addWordAction);
            Ensure.That(addWordResult.MatchedCount).IsNot(0).WithException(_ => new TopicNotFoundException(topic));

            var incrementResult = await topics.UpdateOneAsync(t => t.Name == topic, incrementAction);
            Ensure.That(incrementResult.MatchedCount).IsNot(0).WithException(_ => new TopicNotFoundException(topic));
        }

        public async Task<Word> GetLastWordAsync(string topic)
        {
            var lastWordQuery = topics
               .Find(Builders<MongoTopic>.Filter.Eq(t => t.Name, topic))
               .Project(Builders<MongoTopic>.Projection.Expression(t => t.Words.LastOrDefault()));

            return await lastWordQuery.FirstOrDefaultAsync();
        }

        public async Task<bool> IsBlackListedAsync(string topic, string word)
        {
            var lastWordQuery = topics
               .Find(Builders<MongoTopic>.Filter.Eq(t => t.Name, topic))
               .Project(Builders<MongoTopic>.Projection.Expression(t => t.BlackList.Any(w => w == word)));

            return await lastWordQuery.FirstOrDefaultAsync();
        }

        public async Task AddToBlackListAsync(string topic, string word)
        {
            var addWordAction = Builders<MongoTopic>.Update.Push(t => t.BlackList, word);
            var addWordResult = await topics.UpdateOneAsync(t => t.Name == topic, addWordAction);
            Ensure.That(addWordResult.MatchedCount).IsNot(0).WithException(_ => new TopicNotFoundException(topic));
        }

        public Task DeleteTopicsByAuthorAsync(string author)
        => topics.DeleteManyAsync(t => t.Name == author);


        public Task<bool> WordExistsAsync(string topic, string word, string author)
           => topics.Find(t => t.Name == topic && t.Words.Any(w => w.Value == word && w.Author == author)).AnyAsync();
    }
}
