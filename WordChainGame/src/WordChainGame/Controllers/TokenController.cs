using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WordChainGame.Auth;

namespace WordChainGame.Controllers
{
    public class TokenController : Controller
    {
        private TokenProvider provider;
        private UserManager<User> userManager;

        public TokenController(TokenProvider provider, UserManager<User> userManager)
        {
            this.provider = provider;
            this.userManager = userManager;
        }


        [HttpPost Route("/api/token")]
        public async Task<IActionResult> Post(string username, string password)
        {
            var token = await provider.GenerateToken(username, password);
            return token != null ? Ok(token) as IActionResult : Challenge();
        }

        [HttpDelete Route("/api/token") Authorize]
        public async Task<IActionResult> Delete()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            await userManager.UpdateSecurityStampAsync(user);

            return NoContent();
        }
    }
}
