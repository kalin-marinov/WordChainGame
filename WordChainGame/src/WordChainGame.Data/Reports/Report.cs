using System.ComponentModel.DataAnnotations;

namespace WordChainGame.Data.Models
{
    public class ReportBase
    {
        [Required]
        public string Reporter { get; set; }

        [Required]
        public string Word { get; set; }

        [Required]
        public string Topic { get; set; }

        [Required]
        public string WordAuthor { get; set; }
    }
}
