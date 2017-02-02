using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Auth;
using WordChainGame.Auth.Identity;
using WordChainGame.Data.Reports;
using WordChainGame.Data.Topics;
using WordChainGame.Helpers.Attributes;
using WordChainGame.Models;

namespace WordChainGame.Controllers
{
    [RejectInvalidModel]
    public class UsersController : Controller
    {
        private UserManager<User> manager;
        private IReportsManager reportsManager;
        private ITopicManager topicManager;
        private IIdentityResolver identityResolver;

        public UsersController(UserManager<User> manager, IReportsManager reportsManager,
            ITopicManager topicManager, IIdentityResolver identityResolver)
        {
            this.manager = manager;
            this.reportsManager = reportsManager;
            this.topicManager = topicManager;
            this.identityResolver = identityResolver;
        }

        [HttpPost Route("api/v1/Users")]
        public async Task<IActionResult> Post(RegisterUserModel userData)
        {
            var user = new User(userData.UserName, userData.FullName, userData.Email);
            var result = await manager.CreateAsync(user, userData.Password);

            return result == IdentityResult.Success ? Created("/users/{id}", "Created") as IActionResult
                                                    : BadRequest(GetErrors(result));
        }


        [HttpPatch Route("api/v1/Users") Authorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await manager.FindByNameAsync(User.Identity.Name);
            var result = await manager.ChangePasswordAsync(user, currentPassword, newPassword);

            return PrepareResponse(result);
        }

        [HttpDelete Route("api/v1/Users/{username}"), Authorize]
        public async Task<IActionResult> Delete([FromRoute] string username, [FromForm]string password)
        {
            var user = await manager.FindByNameAsync(username);

            if (await identityResolver.SignInAsync(username, password) == null)
                return BadRequest("Invalid username or password");

            var result = await manager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await topicManager.DeleteTopicsByAuthorAsync(username);
                await reportsManager.DeleteByReporter(username, HttpContext.RequestAborted);
            }

            return PrepareResponse(result);
        }


        private IActionResult PrepareResponse(IdentityResult result)
            => result == IdentityResult.Success ? NoContent() as IActionResult
                                                : BadRequest(GetErrors(result));

        private string GetErrors(IdentityResult result)
            => string.Join(",", result.Errors?.Select(e => e.Description));

    }
}
