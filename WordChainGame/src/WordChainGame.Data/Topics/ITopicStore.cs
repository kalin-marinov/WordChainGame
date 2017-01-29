using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Topics.Models;

namespace WordChainGame.Data
{
    public interface ITopicStore<TopicType> where TopicType : TopicBase
    {
        Task AddWordAsync(string topicName, Word word);

        Task AddTopicAsync(TopicType topic);

        Task DeleteWordAsync(string topic, string word);

        Task<bool> TopicExistsAsync(string topic);

        //Task<IReadOnlyCollection<string>> GetAllTopicNames();

        Task<IReadOnlyCollection<TopicDescription>> GetTopicDescriptions(int skip, int take);

        Task<Word> GetLastWord(string topic);

        Task<IEnumerable<Word>> GetWords(string topic, int skip, int take);

        Task<bool> WordExists(string topic, string word, string author);
    }
}