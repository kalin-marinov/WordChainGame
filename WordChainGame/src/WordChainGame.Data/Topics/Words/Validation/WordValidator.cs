using System;
using System.Text.RegularExpressions;
using WordChainGame.Data.Topics.Words.Validation;

namespace WordChainGame.Data.Topics.Words
{
    public class WordValidator : IWordValidator
    {
        public void Validate(string word)
        {
            if (!Regex.IsMatch(word.ToLower(), "[a-z]+"))
            {
                throw new ArgumentException("The word should consist of english letters only");
            }
        }
    }
}
