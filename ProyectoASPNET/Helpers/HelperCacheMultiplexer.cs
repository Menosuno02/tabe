using StackExchange.Redis;

namespace ProyectoASPNET.Helpers
{
    public class HelperCacheMultiplexer
    {
        private static Lazy<ConnectionMultiplexer>
            CreateConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheRedisKeys = "cacheredistabe.redis.cache.windows.net:6380,password=4AuLp7ljPXAHbv9awttTsAzuSwDohjhIvAzCaAxB1kI=,ssl=True,abortConnect=False";
                return ConnectionMultiplexer.Connect(cacheRedisKeys);
            });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return CreateConnection.Value;
            }
        }
    }
}
