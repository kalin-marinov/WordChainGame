using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System;

namespace WordChainGame
{
    public partial class Startup
    {
        internal  string secretKey => Configuration["Security:SigningKey"];
        internal readonly SymmetricSecurityKey signingKey;

        private IApplicationBuilder app;

        private void ConfigureTokens(IApplicationBuilder app)
        {
            this.app = app;


            // This allows using the token provider as middleware, rather than controller method
            //app.UseSimpleTokenProvider(new TokenProviderOptions
            //{
            //    Path = "/api/token",
            //    Audience = "ExampleAudience",
            //    Issuer = "ExampleIssuer",
            //    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            //});


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
    }
}
