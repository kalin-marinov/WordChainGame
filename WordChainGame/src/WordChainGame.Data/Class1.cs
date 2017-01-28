using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordChainGame.Data
{
    public class Class1
    {
        public Class1()
        {
        }

        public static void Main()
        {
            var client = new MongoClient("mongodb://localhost:27017");

            var db = client.GetDatabase("Test");

            BsonClassMap.RegisterClassMap<Topic>(cm =>
            {
                cm.AutoMap();
            });

            var col = db.GetCollection<Topic>("Topics").AsQueryable().ToList();

            var test = db.GetCollection<Topic>("TestCollection");

            Console.WriteLine("there");
        }
    }

    public class Topic
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public ICollection<Word> Words { get; set;}
    }

    public class Word
    {
        public string Value { get; set; }

        public string Author { get; set; }

        
    }
}
