using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;

namespace WordChainGame.Auth
{
    public class ClaimsFactory : IUserClaimsPrincipalFactory<User>
    {
        private UserManager<User> userManager;

        public ClaimsFactory(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userId = await userManager.GetUserNameAsync(user);
            var userName = await userManager.GetUserIdAsync(user);

            var id = new GenericIdentity(userName, "token");

            id.AddClaim(new Claim(JwtRegisteredClaimNames.NameId, userId));
            id.AddClaim(new Claim(JwtRegisteredClaimNames.GivenName, userName));

            if (userManager.SupportsUserSecurityStamp)
                id.AddClaim(new Claim("SecurityStamp", await userManager.GetSecurityStampAsync(user)));

            return new ClaimsPrincipal(id);
        }
    }
}
