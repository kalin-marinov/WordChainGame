using System.ComponentModel.DataAnnotations;

namespace WordChainGame.Models
{
    public class PaginationModel
    {
        [Range(0, int.MaxValue)]
        public int Skip { get; set; }

        [Range(1, 100)]
        public int Take { get; set; }
    }
}
