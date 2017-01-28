using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordChainGame.Data.Models
{
    public class Topic
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public ICollection<Word> Words { get; set; }
    }

}
