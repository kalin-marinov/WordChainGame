using StackExchange.Redis;
using System.Threading.Tasks;

namespace WordChainGame.Auth
{
    public interface IHashSerailizer<T>
    {
        RedisKey GetKey(T instance);

        HashEntry[] Searlize(T instance);

        T DeSearlize(RedisKey key, IDatabase db);

        Task<T> DeSearlizeAsync(RedisKey key, IDatabase db);
    }
}
