using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WordChainGame.Auth;
using WordChainGame.Helpers.Attributes;
using WordChainGame.Models;

namespace WordChainGame.Controllers
{
    [RejectInvalidModel]
    public class UsersController : Controller
    {
        private UserManager<User> manager;

        public UsersController(UserManager<User> manager)
        {
            this.manager = manager;
        }

        [HttpPost Route("api/Users")]
        public async Task<IActionResult> Post(RegisterUserModel userData)
        {
            var user = new User(userData.UserName, userData.FullName, userData.Email);
            var result = await manager.CreateAsync(user, userData.Password);

            return result == IdentityResult.Success ? Created("/users/{id}", "Created") as IActionResult
                                                    : BadRequest(GetErrors(result));
        }


        [HttpPut Route("api/Users")]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await manager.FindByNameAsync(User.Identity.Name);
            var result = await manager.ChangePasswordAsync(user, currentPassword, newPassword);

            return PrepareResponse(result);
        }

        [HttpDelete Route("api/Users/{username}")]
        public async Task<IActionResult> Delete(string username, string password)
        {
            var user = await manager.FindByNameAsync(username);
            var result = await manager.DeleteAsync(user);

            return PrepareResponse(result);
        }


        private IActionResult PrepareResponse(IdentityResult result)
            => result == IdentityResult.Success ? NoContent() as IActionResult
                                                : BadRequest(GetErrors(result));

        private string GetErrors(IdentityResult result)
            => string.Join(",", result.Errors?.Select(e => e.Description));

    }
}
