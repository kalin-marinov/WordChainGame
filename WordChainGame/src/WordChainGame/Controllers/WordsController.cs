using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WordChainGame.Data.Topics;
using WordChainGame.Helpers.Attributes;
using WordChainGame.Models.Topics;
using WordChainGame.Models.Topics.Words;

namespace WordChainGame.Controllers
{
    [RejectInvalidModel, HandleErrors, Authorize]
    public class WordsController : Controller
    {
        private ITopicManager manager;

        public WordsController(ITopicManager manager)
        {
            this.manager = manager;
        }

        [HttpGet Route("/api/topics/{topic}/words")]
        public async Task<IActionResult> Get(WordsFilter filter)
        {
            var words = await manager.GetWordsAsync(filter.Topic, filter.Skip, filter.Take);
            return Ok(words);
        }

        [HttpPost Route("/api/topics/{topic}/words")]
        public async Task<IActionResult> Post(WordIdentifier word)
        {
            await manager.AddWordAsync(word.Topic, word.Word, User.Identity.Name);
            return NoContent();
        }

        [HttpDelete Route("/api/topics/{topic}/words")]
        public async Task<IActionResult> Delete(WordIdentifier word)
        {
            await manager.DeleteWordAsync(word.Topic, word.Word);
            return NoContent();
        }
    }
}
