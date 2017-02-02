using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WordChainGame.Data.Models;
using WordChainGame.Data.Reports;
using WordChainGame.Helpers.Attributes;

namespace WordChainGame.Controllers
{
    [RejectInvalidModel, HandleErrors, Authorize]
    public class ReportsController : Controller
    {
        private IReportsManager manager;

        private CancellationToken abortToken => HttpContext.RequestAborted;

        public ReportsController(IReportsManager manager)
        {
            this.manager = manager;
        }

        [HttpGet Route("/api/v1/reports"), Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Get()
          => Ok(await manager.GetAll(token: abortToken));


        [HttpPost Route("/api/v1/reports")]
        public async Task<IActionResult> Post(Report report)
        {
            await manager.Add(report, token: abortToken);
            return Created("/reports/", "Report Created");
        }
    }
}
