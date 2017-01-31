using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WordChainGame.Data.Topics;
using WordChainGame.Helpers.Attributes;
using WordChainGame.Models.Topics;

namespace WordChainGame.Controllers
{
    [RejectInvalidModel HandleErrors]
    public class TopicsController : Controller
    {
        private ITopicManager manager;

        public TopicsController(ITopicManager manager)
        {
            this.manager = manager;
        }

        [HttpPost Route("/api/topics")]
        public async Task<IActionResult> Post(CreateTopicModel topic)
        {
            await manager.AddTopicAsync(topic.Name, User.Identity.Name);
            return Created("/topic/{id}", topic.Name);
        }

        [HttpGet Route("/api/topics")]
        public async Task<ActionResult> Get(TopicsFilter filter)
        {
            var allTopics = await manager.GetTopicsAsync(filter.Skip, filter.Take, filter.SortBy);
            return Ok(allTopics);
        }
    }
}
