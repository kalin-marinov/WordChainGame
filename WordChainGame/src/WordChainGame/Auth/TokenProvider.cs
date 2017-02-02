using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WordChainGame.Auth.Identity;

namespace WordChainGame.Auth
{
    public class TokenProvider
    {
        private IIdentityResolver identityResolver;

        public TokenProviderOptions Options { get; private set; }

        public TokenProvider(IOptions<TokenProviderOptions> options, IIdentityResolver identityResolver)
        {
            ThrowIfInvalidOptions(options.Value);
            this.identityResolver = identityResolver;
            this.Options = options.Value;
        }

        public async Task<object> GenerateToken(string username, string password)
        {
            var identity = await identityResolver.SignInAsync(username, password);
            if (identity == null)
            {
                return null;
            }

            string encodedJwt = await PrepareToken(username, identity);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)Options.Expiration.TotalSeconds
            };

            return response;
        }

        private async Task<string> PrepareToken(Microsoft.Extensions.Primitives.StringValues username, ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;

            // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, await Options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64),
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: Options.Issuer,
                audience: Options.Audience,
                claims: claims.Union(identity.Claims),
                notBefore: now,
                expires: now.Add(Options.Expiration),
                signingCredentials: Options.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }

        /// <summary>
        /// Get this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight UTC).
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>Seconds since Unix epoch.</returns>
        public static long ToUnixEpochDate(DateTime date) => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
    }
}
