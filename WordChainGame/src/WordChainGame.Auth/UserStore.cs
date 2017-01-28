using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WordChainGame.Auth
{

    public class UserStore<TUser> : IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>
        where TUser : User
    {
        private bool _disposed;

        private IDatabase db;
        private IHashSerailizer<TUser> seralizer;

        public UserStore(IDatabase db, IHashSerailizer<TUser> seralizer)
        {
            this.db = db;
            this.seralizer = seralizer;
        }

        private RedisKey GetKey(TUser user) => seralizer.GetKey(user);

        private RedisKey GetKey(string userId) => $"Users_{userId}";

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public async Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
           => await db.HashGetAsync(GetKey(user), nameof(user.PasswordHash));

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
           => db.HashExistsAsync(GetKey(user), nameof(user.PasswordHash));

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
           => Task.FromResult(user.UserName);

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
           => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
            => db.HashSetAsync(GetKey(user), nameof(user.UserName), userName);

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedName);

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
             => UpdateAsync(user, cancellationToken);


        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                await db.HashSetAsync(GetKey(user), seralizer.Searlize(user));
                return IdentityResult.Success;
            }
            catch (Exception)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Error storing user" });
            }
        }


        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var field in seralizer.Searlize(user))
                    await db.HashDeleteAsync(GetKey(user), field.Name);


                return IdentityResult.Success;
            }
            catch (Exception)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Error deleting user" });
            }
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
            => FindByNameAsync(userId, cancellationToken);

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await seralizer.DeSearlizeAsync(GetKey(normalizedUserName), db);
            user.NormalizedName = normalizedUserName;
            return user;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
           => db.HashSetAsync(GetKey(user), nameof(user.SecurityStamp), stamp);

        public async Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
           => await db.HashGetAsync(GetKey(user), nameof(user.SecurityStamp));

        public void Dispose()
           => _disposed = true;

    }
}
