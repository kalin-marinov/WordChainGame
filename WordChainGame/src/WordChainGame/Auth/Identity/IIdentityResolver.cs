using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WordChainGame.Auth.Identity
{
    public interface IIdentityResolver
    {
        /// <summary> Signs the user by a given username and password and returns the identity
        /// <para> Returns null if the sign-in operation was not succesful </para> </summary>
        Task<ClaimsIdentity> SignInAsync(string userName, string password);

    }
}
