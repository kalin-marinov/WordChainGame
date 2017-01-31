using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WordChainGame.Models.Topics
{
    public class WordsFilter : PaginationModel
    {
        [Required FromRoute]
        public string Topic { get; set; }

    }
}
