using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Auth;

namespace WordChainGame.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private UserManager<User> manager;

        public LoginController(UserManager<User> manager)
        {
            this.manager = manager;
        }

        public async Task<IActionResult> Post([Required] string userName, string password)
        {
            var user = new User(userName);
            var result = await manager.CreateAsync(user, password);
            var errors = string.Join(",", result.Errors?.Select(e => e.Description));

            return result == IdentityResult.Success ? Ok() as IActionResult
                                                    : BadRequest(errors);
        }
    }
}
