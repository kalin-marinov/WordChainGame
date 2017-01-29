using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data.Mongo.Models;
using Xunit;

namespace WordChainGame.Data.Tests
{
    public class ReportsStoreTests
    {
        MongoClient client;
        IMongoDatabase db;
        ReportStore store;

        public ReportsStoreTests()
        {
            client = new MongoClient("mongodb://admin:admin@ds135029.mlab.com:35029/wordgame");
            db = client.GetDatabase("wordgame");
            store = new ReportStore(db);

            db.DropCollection("Reports");
            db.CreateCollection("Reports");
        }

        [Fact]
        public async Task CanManageReports()
        {
            await store.Add(new MongoReport
            {
                Reporter = "someBody",
                Word = "someWord",
                WordAuthor = "someBody",
                Topic = "test"
            });

            await store.Add(new MongoReport
            {
                Reporter = "someBody2",
                Word = "someWord2",
                WordAuthor = "someBody2",
                Topic = "test2"
            });

            var reports = await store.GetAll();

            Assert.Equal(2, reports.Count);
            Assert.Equal("someBody2", reports.Last().Reporter);
            Assert.Equal("test2", reports.Last().Topic);
            Assert.Equal("someBody", reports.First().WordAuthor);
            Assert.Equal("someWord", reports.First().Word);
        }
    }
}
