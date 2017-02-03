using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WordChainGame.Auth;

namespace WordChainGame.Helpers.Attributes
{
    public class VerifySecurityStampAttribute : ActionFilterAttribute
    {
        private UserManager<User> userManager;

        public VerifySecurityStampAttribute(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            var stamp = user.FindFirst("SecurityStamp").Value;

            var dbUser = userManager.FindByNameAsync(user.Identity.Name).Result;

            if (dbUser.SecurityStamp != stamp)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
