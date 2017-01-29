using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WordChainGame.Data.Mongo.Models;
using WordChainGame.Data.Reports;

namespace WordChainGame.Controllers
{
    public class ReportsController : Controller
    {
        private IReportsManager<MongoReport> manager;

        public ReportsController(IReportsManager<MongoReport> manager)
        {
            this.manager = manager;
        }

        public async Task<IActionResult> Get()
          => Ok(await manager.GetAll(HttpContext.RequestAborted));

        public async Task<IActionResult> Post(MongoReport report)
        {
            await manager.Add(report, HttpContext.RequestAborted);
            return NoContent();
        }
    }
}
