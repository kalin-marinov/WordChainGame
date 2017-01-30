using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Mongo.Models;
using WordChainGame.Data.Reports;
using WordChainGame.Helpers.Attributes;

namespace WordChainGame.Controllers
{
    [RejectInvalidModel, HandleErrors, Authorize]
    public class ReportsController : Controller
    {
        private IReportsManager manager;

        public ReportsController(IReportsManager manager)
        {
            this.manager = manager;
        }

        public async Task<IActionResult> Get()
          => Ok(await manager.GetAll(HttpContext.RequestAborted));

        public async Task<IActionResult> Post(Report report)
        {
            await manager.Add(report, HttpContext.RequestAborted);
            return NoContent();
        }
    }
}
