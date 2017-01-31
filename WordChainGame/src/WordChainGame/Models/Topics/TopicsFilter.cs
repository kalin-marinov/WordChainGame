using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data.Topics;

namespace WordChainGame.Models.Topics
{
    public class TopicsFilter : PaginationModel
    {
        public TopicSortCriteria SortBy { get; set; }

    }
}
