using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace WordChainGame.Auth.Hashing
{
    public class UserHasher : IHashSerailizer<User>
    {
        public User DeSearlize(RedisKey key, IDatabase db)
        {
            if (db.HashLength(key) == 0)
                return null;

            return new User
            {
                UserName = db.HashGet(key, nameof(User.UserName)),
                IsAdmin = (bool)db.HashGet(key, nameof(User.IsAdmin)),
                PasswordHash = db.HashGet(key, nameof(User.PasswordHash)),
                SecurityStamp = db.HashGet(key, nameof(User.SecurityStamp)),
                Email = db.HashGet(key, nameof(User.Email)),
                FullName = db.HashGet(key, nameof(User.FullName))
            };
        }

        public async Task<User> DeSearlizeAsync(RedisKey key, IDatabase db)
        {
            if (await db.HashLengthAsync(key) == 0)
                return null;

            return new User
            {
                UserName = await db.HashGetAsync(key, nameof(User.UserName)),
                IsAdmin = (bool)await db.HashGetAsync(key, nameof(User.IsAdmin)),
                PasswordHash = await db.HashGetAsync(key, nameof(User.PasswordHash)),
                SecurityStamp = await db.HashGetAsync(key, nameof(User.SecurityStamp)),
                Email = await db.HashGetAsync(key, nameof(User.Email)),
                FullName = await db.HashGetAsync(key, nameof(User.FullName)),
            };
        }

        public RedisKey GetKey(User instance)
            => $"Users_{instance.NormalizedName}";

        public HashEntry[] Searlize(User instance)
        {
            return new[]
            {
                new HashEntry(nameof(instance.UserName), instance.UserName),
                new HashEntry(nameof(instance.PasswordHash), instance.PasswordHash),
                new HashEntry(nameof(instance.IsAdmin), instance.IsAdmin),
                new HashEntry(nameof(instance.SecurityStamp), instance.SecurityStamp),
                new HashEntry(nameof(instance.FullName), instance.FullName),
                new HashEntry(nameof(instance.Email), instance.Email)
            };
        }
    }
}
