using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Mongo.Models;
using WordChainGame.Data.Reports;

namespace WordChainGame.Data
{
    public class ReportStore : IReportStore<MongoReport>
    {
        private IMongoDatabase database;

        private IMongoCollection<MongoReport> reports => database.GetCollection<MongoReport>("Reports");

        public ReportStore(IMongoDatabase database)
        {
            this.database = database;
        }

        public Task Add(MongoReport report, CancellationToken token = default(CancellationToken))
            => reports.InsertOneAsync(report, new InsertOneOptions { BypassDocumentValidation = false }, token);

        public async Task<IReadOnlyCollection<MongoReport>> GetAll(CancellationToken token = default(CancellationToken))
            => await reports.AsQueryable().ToListAsync(token);
    }
}
