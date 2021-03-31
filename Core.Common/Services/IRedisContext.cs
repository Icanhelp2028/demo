using StackExchange.Redis;

namespace Core.Common.Services
{
    /// <summary>Redis</summary>
    public interface IRedisContext
    {
        /// <summary>数据存取</summary>
        IDatabase Database { get; }

        /// <summary>数据订阅/发布</summary>
        ISubscriber Subscriber { get; }
    }
}