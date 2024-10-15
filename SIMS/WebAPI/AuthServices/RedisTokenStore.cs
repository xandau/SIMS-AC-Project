using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace WebAPI.AuthServices
{
    public class RedisTokenStore
    {
        private readonly IDatabase _storage;

        public RedisTokenStore()
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("ConnectionStrings:REDIS", out var secretData);

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(secretData);
            _storage = redis.GetDatabase();
        }

        public async Task StoreRefreshTokenAsync(long userId, string username, string refresh_token)
        {
            var hash = new HashEntry[]
            {
                new HashEntry("userId", userId),
                new HashEntry("username", username)
            } ;

            await _storage.HashSetAsync(refresh_token, hash);
        }

        public async Task<HashEntry[]> GetUserFromRefreshToken(string refresh_token)
        {
            return await _storage.HashGetAllAsync(refresh_token);
        }

        public async Task<bool> RemoveRefreshTokenAsync(string refresh_token)
        {
            return await _storage.KeyDeleteAsync(refresh_token);
        }
    }
}
