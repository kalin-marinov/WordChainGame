using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WordChainGame.Models.Topics.Words
{
    public class WordIdentifier
    {
        [Required FromRoute]
        public string Topic { get; set; }

        [Required]
        public string Word { get; set; }

    }
}
