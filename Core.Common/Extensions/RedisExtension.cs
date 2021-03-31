using StackExchange.Redis;
using System;
using System.Text.Json;

namespace Core.Common.Extensions
{
    public static class RedisExtension
    {
        public static void SetObject<T>(this IDatabase database, string key, T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var str = JsonSerializer.Serialize(value);
            database.StringSet(key, str, expiry, when, flags);
        }

        public static T GetObject<T>(this IDatabase database, string key, CommandFlags flags = CommandFlags.None)
        {
            var str = (string)database.StringGet(key, flags);
            return str == null ? default : JsonSerializer.Deserialize<T>(str);
        }

        public static void SetValue<T>(this IDatabase database, string key, T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : IFormattable => database.StringSet(key, value.ToString(), expiry, when, flags);

        public static T GetValue<T>(this IDatabase database, string key, CommandFlags flags = CommandFlags.None)
            where T : IFormattable
        {
            var str = (string)database.StringGet(key, flags);
            return str == null ? default : (T)Convert.ChangeType(str, typeof(T));
        }
    }
}