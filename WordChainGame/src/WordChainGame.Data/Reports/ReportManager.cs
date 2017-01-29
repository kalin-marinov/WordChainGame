using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WordChainGame.Data.Models;

namespace WordChainGame.Data.Reports
{
    public class ReportManager<TReport, TTopic> : IReportsManager<TReport>
        where TReport : ReportBase
        where TTopic : TopicBase
    {
        IReportStore<TReport> reportStore;
        ITopicStore<TTopic> topicStore;

        public ReportManager(IReportStore<TReport> reportStore, ITopicStore<TTopic> topicStore)
        {
            this.reportStore = reportStore;
            this.topicStore = topicStore;
        }

        public async Task Add(TReport report, CancellationToken token = default(CancellationToken))
        {
            if (!(await topicStore.TopicExistsAsync(report.Topic)))
                throw new ArgumentException("topic does not exist");

            if (!(await topicStore.WordExists(report.Topic, report.Word, report.WordAuthor)))
                throw new ArgumentException($"No word {report.Word} was posted in {report.Topic} by {report.WordAuthor}");

            await reportStore.Add(report, token);
        }

        public Task<IReadOnlyCollection<TReport>> GetAll(CancellationToken token = default(CancellationToken))
            => reportStore.GetAll(token);
    }
}
