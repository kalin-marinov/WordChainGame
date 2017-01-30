using System;

namespace WordChainGame.Data.Exceptions
{
    public class TopicNotFoundException : Exception
    {
        public string Topic { get; private set; }

        public TopicNotFoundException(string topic) : this(topic, $"Topic {topic} was not found!")
        {
            this.Topic = topic;
        }

        public TopicNotFoundException(string topic, string message) : base(message)
        {
            this.Topic = topic;
        }

        public TopicNotFoundException(string topic, string message, Exception inner) : base(message, inner)
        {
            this.Topic = topic;
        }

    }
}
