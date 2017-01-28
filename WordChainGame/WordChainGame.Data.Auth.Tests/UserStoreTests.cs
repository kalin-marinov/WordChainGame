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
                PasswordHash = "hash(password)",
                SecurityStamp = Guid.NewGuid().ToString()
            }, token);

           var user = await store.FindByNameAsync("TEST", token);

            Assert.Equal("test", user.UserName);
            Assert.Equal("hash(password)", user.PasswordHash);
        }

        [Fact]
        public async Task CanDeleteUsers()
        {
            await store.CreateAsync(new User
            {
                UserName = "test",
                NormalizedName = "TEST",
                PasswordHash = "hash(password)",
                SecurityStamp = Guid.NewGuid().ToString()
            }, token);

            var user = await store.FindByNameAsync("TEST", token);
            user.NormalizedName = "TEST"; // Simulating the user manager work (i.e. setting normalized names constantly and password hashes)
            await store.DeleteAsync(user, token);

            user = await store.FindByNameAsync("TEST", token);

            Assert.Null(user);
        }
    }
}
