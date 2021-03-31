using Core.Common.Services;
using Core.Common.Generic;
using StackExchange.Redis;
using System;

namespace Core.Data.Services
{
    /// <summary>流水号生成</summary>
    public sealed class SequenceService : ISequenceService
    {
        private static readonly DateTime StartTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IDatabase _database;

        public SequenceService(IRedisContext redisContext) => _database = redisContext.Database;

        /// <summary>由时间生成的唯一流水号</summary>
        public long GenerateSequence(int count = 1)
        {
            var seed = 200000L;
            var timestamp = ((DateTime.Now.ToUniversalTime() - StartTime).Ticks / seed);

            if (!_database.LockTake(RedisKeys.User_Lock_SequenceKey, "", TimeSpan.FromSeconds(5)))
            {
                string message = $"Sequence Timeout:{RedisKeys.User_Sequences_SequenceKey}";
                throw new Exception(message);
            }

            string sequence = _database.StringGet(RedisKeys.User_Sequences_SequenceKey);
            if (sequence != null)
            {
                int oldTimeStamp = int.Parse(sequence);
                if (timestamp <= oldTimeStamp)
                {
                    timestamp = oldTimeStamp + 1;
                }
            }

            _database.StringSet(RedisKeys.User_Sequences_SequenceKey, (timestamp + count - 1L).ToString());
            _database.LockRelease(RedisKeys.User_Lock_SequenceKey, "");

            return timestamp;
        }

        /// <summary>由递增生成的唯一id</summary>
        public long GenerateIdentity(int count = 1)
        {
            var value = _database.StringIncrement(RedisKeys.User_Sequences_IdentityKey, count);

            return value - count - 1;
        }
    }
}