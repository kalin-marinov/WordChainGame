using System;
using System.Text.RegularExpressions;
using WordChainGame.Data.Topics.Validation;

namespace WordChainGame.Data.Topics
{
    public class TopicValidator: ITopicValidator
    {
        public void Validate(string topicName)
        {
            if(!Regex.IsMatch(topicName.ToLower(), @"\w+[\s*\w+]*"))
            {
                throw new ArgumentException("Please enter a topic that consists of words and numbers only");
            }
        }

    }
}
