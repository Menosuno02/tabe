using StackExchange.Redis;

namespace ProyectoASPNET.Helpers
{
    public class HelperCacheMultiplexer
    {
        private static Lazy<ConnectionMultiplexer>
            CreateConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheRedisKeys = "cacheredistabe.redis.cache.windows.net:6380,password=F9ovdQJxxDLGkAb4xlZUb5Ay0VVYX0SaKAzCaIvnnc4=,ssl=True,abortConnect=False";
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
