using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

        [HttpGet Route("/api/v1/topics/{topic}/words")]
        public async Task<IActionResult> Get(WordsFilter filter)
        {
            var words = await manager.GetWordsAsync(filter.Topic, filter.Skip, filter.Take);
            return Ok(words);
        }

        [HttpPost Route("/api/v1/topics/{Topic}/words")]
        public async Task<IActionResult> Post(WordIdentifier word)
        {
            await manager.AddWordAsync(word.Topic, word.Word, User.Identity.Name);
            return NoContent();
        }

        [HttpDelete Route("/api/v1/topics/{topic}/words/{word}") Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete([Required FromRoute] string topic, [Required FromRoute] string word)
        {
            await manager.DeleteWordAsync(topic, word);
            return NoContent();
        }
    }
}
