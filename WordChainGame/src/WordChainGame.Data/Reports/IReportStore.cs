using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WordChainGame.Data.Models;

namespace WordChainGame.Data.Reports
{
    public interface IReportStore
    {
        Task Add(Report report, CancellationToken token = default(CancellationToken));

        Task<IReadOnlyCollection<Report>> GetAll(CancellationToken token = default(CancellationToken));

        Task DeleteByReporter(string reporter, CancellationToken token = default(CancellationToken));
    }
}
