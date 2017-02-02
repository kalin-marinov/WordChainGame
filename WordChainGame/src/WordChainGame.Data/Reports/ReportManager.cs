using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WordChainGame.Data.Exceptions;
using WordChainGame.Data.Models;

namespace WordChainGame.Data.Reports
{
    public class ReportManager : IReportsManager
    {
        IReportStore reportStore;
        ITopicStore topicStore;

        public ReportManager(IReportStore reportStore, ITopicStore topicStore)
        {
            this.reportStore = reportStore;
            this.topicStore = topicStore;
        }

        public async Task Add(Report report, CancellationToken token = default(CancellationToken))
        {
            if (!(await topicStore.TopicExistsAsync(report.Topic)))
                throw new TopicNotFoundException(report.Topic);

            if (!(await topicStore.WordExistsAsync(report.Topic, report.Word, report.WordAuthor)))
                throw new WordNotFoundException(report.Topic, $"No word {report.Word} was posted in {report.Topic} by {report.WordAuthor}");

            await reportStore.Add(report, token);
        }

        public Task DeleteByReporter(string reporter, CancellationToken token = default(CancellationToken))
             => reportStore.DeleteByReporter(reporter, token);

        public Task<IReadOnlyCollection<Report>> GetAll(CancellationToken token = default(CancellationToken))
            => reportStore.GetAll(token);
    }
}
