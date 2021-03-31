//using Core.Common.Generic;
//using Core.Common.Services;
//using StackExchange.Redis;
//using System;

//namespace Core.Data.Services
//{
//    public class HubConnectionService : IHubConnectionService
//    {
//        private readonly IDatabase database;

//        public HubConnectionService(IRedisContext _redisContext)
//        {
//            database = _redisContext.Database;
//        }

//        public void AddConnection(string userName, string connectionId)
//        {
//            database.ListRightPush(RedisKeys.Chat_Connection + userName, connectionId);
//            Breath(userName);
//        }

//        public void RemoveConnection(string userName, string connectionId)
//        {
//            database.ListRemove(RedisKeys.Chat_Connection + userName, connectionId);
//        }

//        public string[] GetConnections(string userName)
//        {
//            var connnections = database.ListRange(RedisKeys.Chat_Connection + userName);
//            var array = new string[connnections.Length];

//            for (int i = 0, c = connnections.Length; i < c; i++)
//                array[i] = connnections[i];

//            return array;
//        }

//        /// <summary>
//        /// 心跳包，需要客户端定时调用
//        /// </summary>
//        public void Breath(string userName)
//        {
//            database.KeyExpire(RedisKeys.Chat_Connection + userName, TimeSpan.FromMinutes(1));
//        }
//    }
//}