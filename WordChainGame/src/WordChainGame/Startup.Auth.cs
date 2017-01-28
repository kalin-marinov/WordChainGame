using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using WordChainGame.Auth;
using Microsoft.AspNetCore.Identity;

namespace WordChainGame
{
    public partial class Startup
    {
        // The secret key every token will be signed with.
        // Keep this safe on the server!
        private static readonly string secretKey = "mysupersecret_secretkey!123";
        private IApplicationBuilder app;

        private void ConfigureAuth(IApplicationBuilder app)
        {
            this.app = app;
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity
            });


            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var signInManger = app.ApplicationServices.GetService(typeof(SignInManager<User>)) as SignInManager<User>;
            var userManager = app.ApplicationServices.GetService(typeof(UserManager<User>)) as UserManager<User>;

            var user = await userManager.FindByNameAsync(username);

            // Don't do this in production, obviously!
            if ((await signInManger.CheckPasswordSignInAsync(user, password, lockoutOnFailure:false)) == SignInResult.Success)
            {
                return new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { });
            }

            return null;
        }
    }
}
