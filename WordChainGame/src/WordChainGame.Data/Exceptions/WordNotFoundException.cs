using System;

namespace WordChainGame.Data.Exceptions
{
    public class WordNotFoundException : Exception
    {
        public string Word { get; private set; }

        public WordNotFoundException(string topic)
        {
            this.Word = topic;
        }

        public WordNotFoundException(string topic, string message) : base(message)
        {
            this.Word = topic;
        }

        public WordNotFoundException(string topic, string message, Exception inner) : base(message, inner)
        {
            this.Word = topic;
        }

    }
}
