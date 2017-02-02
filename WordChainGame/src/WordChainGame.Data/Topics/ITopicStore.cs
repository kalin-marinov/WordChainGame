using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Topics.Models;

namespace WordChainGame.Data
{
    public interface ITopicStore
    {
        Task AddTopicAsync(string name, string author);

        Task<bool> TopicExistsAsync(string topic);

        Task DeleteTopicsByAuthorAsync(string author);

        Task<IReadOnlyCollection<TopicDescription>> GetTopicsAsync(int skip, int take, string sortField);

        Task<Word> GetLastWordAsync(string topic);

        Task<IEnumerable<Word>> GetWordsAsync(string topic, int skip, int take);

        Task AddWordAsync(string topicName, Word word);

        Task DeleteWordAsync(string topic, string word);

        Task<bool> WordExistsAsync(string topic, string word, string author);

        Task<bool> IsBlackListedAsync(string topic, string word);

        Task AddToBlackListAsync(string topic, string word);
    }
}