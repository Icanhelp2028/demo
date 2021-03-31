using Core.Common.Services;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Core.Data.Services
{
    /// <summary>Redis</summary>
    public sealed class RedisContext : IRedisContext
    {
        public IDatabase Database { get; }

        public ISubscriber Subscriber { get; }

        public RedisContext(IConfigurationSection configurationSection)
        {
            var connectionString = configurationSection.GetValue<string>("ConnectionString");
            var dbIndex = configurationSection.GetValue<int>("DbIndex");

            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            Database = connectionMultiplexer.GetDatabase(dbIndex);
            Subscriber = connectionMultiplexer.GetSubscriber();
        }
    }
}