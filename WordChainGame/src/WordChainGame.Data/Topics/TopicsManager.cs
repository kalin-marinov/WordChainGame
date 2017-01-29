﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Topics;
using WordChainGame.Data.Topics.Validation;
using WordChainGame.Data.Topics.Words.Validation;

namespace WordChainGame.Data
{
    public class TopicsManager<TTopic> : ITopicManager<TTopic>
        where TTopic : TopicBase, new()
    {
        ITopicStore<TTopic> store;
        IWordValidator wordValidator;
        ITopicValidator topicValidator;

        public TopicsManager(ITopicStore<TTopic> store, ITopicValidator topicValidator, IWordValidator wordValidator)
        {
            this.store = store;
            this.topicValidator = topicValidator;
            this.wordValidator = wordValidator;
        }

        public async Task AddTopicAsync(string topic)
        {
            topicValidator.Validate(topic);

            if (await store.TopicExistsAsync(topic))
                throw new ArgumentException("Topic already exists", nameof(topic));

            var newTopic = new TTopic
            {
                Name = topic,
                Words = new List<Word>()
            };

            await store.AddTopicAsync(newTopic);
        }

        public async Task<IReadOnlyCollection<string>> GetTopicNamesAsync(int skip, int take, TopicSortCriteria sortBy)
        {
            switch (sortBy)
            {
                case TopicSortCriteria.Name:
                    return await store.GetTopicNames(skip, take, t => t.Name);
                case TopicSortCriteria.Count:
                    return (await store.GetTopicNames(skip, take, null));
                default:
                    throw new ArgumentException();
            }
        }

        public async Task<IEnumerable<Word>> GetWordsAsync(string topic, int skip, int take)
        {
            topicValidator.Validate(topic);
            return await store.GetWords(topic, skip, take);
        }

        public async Task AddWordAsync(string topic, string word, string author)
        {
            topicValidator.Validate(topic);
            wordValidator.Validate(word);

            var lastWord = await store.GetLastWord(topic);

            if (lastWord.Value.Last() != word.First())
                throw new ArgumentException("The new word must start with the last letter of the previous one", nameof(word));

            var newWord = new Word { Author = author, Value = word };
            await store.AddWordAsync(topic, newWord);
        }

        public async Task DeleteWordAsync(string topic, string word)
        {
            topicValidator.Validate(topic);
            wordValidator.Validate(word);

            await store.DeleteWordAsync(topic, word);
        }
    }
}