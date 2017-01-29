using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Data;
using WordChainGame.Data.Mongo.Models;
using WordChainGame.Data.Topics;

namespace WordChainGame.Controllers
{
    public class WordsController : Controller
    {
        private ITopicManager<MongoTopic> manager;

        public WordsController(ITopicManager<MongoTopic> manager)
        {
            this.manager = manager;
        }

        [HttpGet Route("/api/topics/{topic}/words")]
        public async Task<IActionResult> Get(string topic, int skip, int take)
        {
            var words = await manager.GetWordsAsync(topic, skip, take);
            return Ok(words);
        }

        [HttpPost Route("/api/topics/{topic}/words")]
        public async Task<IActionResult> Post(string topic, string word)
        {
            await manager.AddWordAsync(topic, word, User.Identity.Name);
            return NoContent();
        }

        [HttpDelete Route("/api/topics/{topic}/words")]
        public async Task<IActionResult> Delete(string topic, string word)
        {
            await manager.DeleteWordAsync(topic, word);
            return NoContent();
        }
    }
}
