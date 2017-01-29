using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using WordChainGame.Auth;
using WordChainGame.Auth.Hashing;
using Xunit;

namespace WordChainGame.Data.Auth.Tests
{
    public class UserStoreTests
    {
        static CancellationToken token = default(CancellationToken);
        private UserStore<User> store;

        public UserStoreTests()
        {
            var multiPlexer = ConnectionMultiplexer.Connect("localhost:6379");
            var db = multiPlexer.GetDatabase();
            store = new UserStore<User>(db, new UserHasher());
        }

        [Fact]
        public async Task CanManageUsers()
        {
            await store.CreateAsync(new User
            {
                UserName = "test",
                NormalizedName = "TEST",
                Email = "test@test.test",
                FullName = "tester",
                PasswordHash = "hash(password)",
                SecurityStamp = Guid.NewGuid().ToString()
            }, token);

            var user = await store.FindByNameAsync("TEST", token);

            Assert.Equal("test", user.UserName);
            Assert.Equal("test@test.test", user.Email);
        }

        [Fact]
        public async Task CanDeleteUsers()
        {
            var result = await store.CreateAsync(new User
            {
                UserName = "test",
                Email = "test@test.test",
                FullName = "tester",
                NormalizedName = "TEST",
                PasswordHash = "hash(password)",
                SecurityStamp = Guid.NewGuid().ToString()
            }, token);

            var user = await store.FindByNameAsync("TEST", token);
            await store.DeleteAsync(user, token);

            user = await store.FindByNameAsync("TEST", token);

            Assert.Null(user);
        }
    }
}
