using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Topics;
using WordChainGame.Data.Topics.Models;
using WordChainGame.Data.Topics.Validation;
using WordChainGame.Data.Topics.Words.Validation;

namespace WordChainGame.Data
{
    public class TopicsManager : ITopicManager
    {
        ITopicStore store;
        IWordValidator wordValidator;
        ITopicValidator topicValidator;

        public TopicsManager(ITopicStore store, ITopicValidator topicValidator, IWordValidator wordValidator)
        {
            this.store = store;
            this.topicValidator = topicValidator;
            this.wordValidator = wordValidator;
        }

        public async Task AddTopicAsync(string topic, string author)
        {
            topicValidator.Validate(topic);

            if (await store.TopicExistsAsync(topic))
                throw new ArgumentException("Topic already exists", nameof(topic));

            await store.AddTopicAsync(topic, author);
        }

        public async Task<IReadOnlyCollection<TopicDescription>> GetTopicsAsync(int skip, int take, TopicSortCriteria sortBy)
        {
            switch (sortBy)
            {
                case TopicSortCriteria.Name:
                    return await store.GetTopicsAsync(skip, take, "Name");
                case TopicSortCriteria.Count:
                    return await store.GetTopicsAsync(skip, take, "WordCount");
                default:
                    throw new ArgumentException();
            }
        }

        public async Task<IEnumerable<Word>> GetWordsAsync(string topic, int skip, int take)
        {
            topicValidator.Validate(topic);
            return await store.GetWordsAsync(topic, skip, take);
        }

        public async Task AddWordAsync(string topic, string word, string author)
        {
            topicValidator.Validate(topic);
            wordValidator.Validate(word);

            if (await store.IsBlackListedAsync(topic, word))
                throw new ArgumentException($"The word {word} is black-listed for topic {topic}", nameof(word));

            var lastWord = await store.GetLastWordAsync(topic);

            if (lastWord != null && lastWord.Value.Last() != word.First())
                throw new ArgumentException("The new word must start with the last letter of the previous one", nameof(word));

            var newWord = new Word { Author = author, Value = word };
            await store.AddWordAsync(topic, newWord);
        }

        public async Task DeleteWordAsync(string topic, string word)
        {
            topicValidator.Validate(topic);
            wordValidator.Validate(word);

            await store.DeleteWordAsync(topic, word);
            await store.AddToBlackListAsync(topic, word);
        }

        public  Task DeleteTopicsByAuthorAsync(string author)
            => store.DeleteTopicsByAuthorAsync(author);

    }
}
