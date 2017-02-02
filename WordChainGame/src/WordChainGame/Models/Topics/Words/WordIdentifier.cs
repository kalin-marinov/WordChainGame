using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WordChainGame.Models.Topics.Words
{
    [ModelBinder(Name = "")]
    public class WordIdentifier
    {
        [Required FromRoute]
        public string Topic { get; set; }

        [Required FromForm]
        public string Word { get; set; }

    }
}
