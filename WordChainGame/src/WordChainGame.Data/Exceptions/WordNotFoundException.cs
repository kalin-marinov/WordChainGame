using System;

namespace WordChainGame.Data.Exceptions
{
    public class WordNotFoundException : Exception
    {
        public string Word { get; private set; }

        public string Topic { get; private set; }

        public WordNotFoundException(string word, string topic) : base($"word {word} was not found in topic {topic}")
        {
            this.Word = word;
            this.Topic = topic;
        }

        public WordNotFoundException(string word, string topic, string message) : base(message)
        {
            this.Word = word;
            this.Topic = topic;
        }

        public WordNotFoundException(string word, string topic, string message, Exception inner) : base(message, inner)
        {
            this.Word = word;
            this.Topic = topic;
        }

    }
}
