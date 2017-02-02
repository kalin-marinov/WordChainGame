using System.Collections.Generic;

namespace WordChainGame.Data.Models
{
    public class TopicBase
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public virtual ICollection<Word> Words { get; set; }

        public virtual ICollection<string> BlackList { get; set; }
    }

}
