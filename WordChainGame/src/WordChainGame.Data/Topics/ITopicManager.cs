using System.Collections.Generic;
using System.Threading.Tasks;
using WordChainGame.Data.Models;

namespace WordChainGame.Data.Topics
{
    public interface ITopicManager<TTopic> where TTopic : TopicBase
    {
        Task AddTopicAsync(string topic);

        Task<IReadOnlyCollection<string>> GetTopicNamesAsync(int skip, int take, TopicSortCriteria sortBy);

        Task<IEnumerable<Word>> GetWordsAsync(string topic, int skip, int take);

        Task AddWordAsync(string topic, string word, string author);

        Task DeleteWordAsync(string topic, string word);
    }
}
