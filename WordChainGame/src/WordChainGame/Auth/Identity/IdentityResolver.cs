using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace WordChainGame.Auth.Identity
{
    public class IdentityResolver : IIdentityResolver
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public IdentityResolver(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<ClaimsIdentity> SignInAsync(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return null;

            if ((await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false)) == SignInResult.Success)
            {
                return new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[]
                {
                     new Claim(JwtRegisteredClaimNames.Email, user.Email),
                     new Claim("admin", user.IsAdmin.ToString()),
                     new Claim("secStamp", user.SecurityStamp)
                });
            }

            return null;
        }
    }
}
