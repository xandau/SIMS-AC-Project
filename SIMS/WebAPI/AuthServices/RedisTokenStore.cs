using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace WebAPI.AuthServices
{
    public class RedisTokenStore
    {
        private readonly IDatabase _storage;

        public RedisTokenStore()
        {
#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? endpointURL = config["ConnectionStrings-REDIS"];
            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints = { endpointURL },
                Ssl = true
            };
#else
            IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            string? endpointURL = config["ConnectionStrings-REDIS"];

            var redisOptions = new ConfigurationOptions
            {
                EndPoints = { endpointURL },
                /*
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                */
                AllowAdmin = true,
                Ssl = true
            };
#endif
            if (string.IsNullOrEmpty(endpointURL)) {
                throw new ArgumentNullException(nameof(endpointURL), "Redis connection endpoint cannot be null or empty.");
            }
            Console.WriteLine("Redis-ConnectionString: " + endpointURL);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisOptions);
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
