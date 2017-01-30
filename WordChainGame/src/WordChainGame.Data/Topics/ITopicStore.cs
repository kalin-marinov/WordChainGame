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
        Task AddWordAsync(string topicName, Word word);

        Task AddTopicAsync(string name, string author);

        Task DeleteWordAsync(string topic, string word);

        Task<bool> TopicExistsAsync(string topic);

        Task<IReadOnlyCollection<TopicDescription>> GetTopicDescriptions(int skip, int take, string sortField);

        Task<Word> GetLastWord(string topic);

        Task<IEnumerable<Word>> GetWords(string topic, int skip, int take);

        Task<bool> WordExists(string topic, string word, string author);
    }
}