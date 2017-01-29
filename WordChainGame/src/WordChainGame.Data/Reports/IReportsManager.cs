using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WordChainGame.Data.Models;

namespace WordChainGame.Data.Reports
{
    public interface IReportsManager<TReport>
        where TReport : ReportBase
    {
        Task Add(TReport report, CancellationToken token = default(CancellationToken));

        Task<IReadOnlyCollection<TReport>> GetAll(CancellationToken token = default(CancellationToken));
    }
}
