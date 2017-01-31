using System.ComponentModel.DataAnnotations;

namespace WordChainGame.Models.Topics
{
    public class CreateTopicModel
    {
        [Required]
        public string Name { get; set; }

    }
}
