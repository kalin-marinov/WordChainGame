using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WordChainGame.Data.Models;

namespace WordChainGame.Data.Mongo.Models
{
    public class MongoTopic : TopicBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

    }
}
