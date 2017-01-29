using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WordChainGame.Data.Mongo.Models;
using WordChainGame.Data.Topics;
using WordChainGame.Helpers.Attributes;

namespace WordChainGame.Controllers
{
    [HandleErrors]
    public class TopicsController : Controller
    {
        private ITopicManager<MongoTopic> manager;

        public TopicsController(ITopicManager<MongoTopic> manager)
        {
            this.manager = manager;
        }

        [HttpPost Route("/api/topics")]
        public async Task<IActionResult> Post(string name)
        {
            await manager.AddTopicAsync(name, User.Identity.Name);
            return Created("/topic/{id}", name);
        }

        [HttpGet Route("/api/topics")]
        public async Task<ActionResult> Get(int skip, int take, TopicSortCriteria sortBy)
        {
            var allTopics = await manager.GetTopicsAsync(skip, take, sortBy);
            return Ok(allTopics);
        }
    }
}
