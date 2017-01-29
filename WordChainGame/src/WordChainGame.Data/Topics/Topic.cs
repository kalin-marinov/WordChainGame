using System.Collections.Generic;

namespace WordChainGame.Data.Models
{
    public class TopicBase
    {
        public string Name { get; set; }


        public virtual ICollection<Word> Words { get; set; }
    }

}
