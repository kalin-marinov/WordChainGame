using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordChainGame.Data
{
    public class TopicsManager
    {
        private IMongoDatabase database;

        public TopicsManager(IMongoDatabase database)
        {
            this.database = database;
        }

        public void AddTopic(string name)
        {
            var topic = new Topic { Name = name };
            var topics = database.GetCollection<Topic>("Topics");
            topics.InsertOne(topic);
        }

        public IEnumerable<Word> GetWords(string topic)
        {
            var topics = database.GetCollection<Topic>("Topics");
            var topicData = topics.Find(t => t.Name == topic).Single();

            return topicData.Words;
        }

        public void AddWord(string topic, string word, string author)
        {
            var topics = database.GetCollection<Topic>("Topics");
            var topicData = topics.Find(t => t.Name == topic).Single();
            var lastWord = topicData.Words.Last().Value;

            if (word.Last() != lastWord.First())
            {
                throw new ArgumentException("the new word must start with the last letter of the first word");
            }

            topicData.Words.Add(new Word { Value = word, Author = author });
            topics.UpdateOne(t => t.Name == topic, topic);
        }
    }
}
