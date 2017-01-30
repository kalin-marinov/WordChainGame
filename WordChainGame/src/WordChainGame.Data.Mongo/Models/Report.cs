using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WordChainGame.Data.Models;

namespace WordChainGame.Data.Mongo.Models
{
    public class MongoReport : Report
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public static MongoReport FromBase(Report report)
        {
            return new MongoReport
            {
                Reporter = report.Reporter,
                Topic = report.Topic,
                Word = report.Word,
                WordAuthor = report.WordAuthor
            }; 
        }

    }
}
