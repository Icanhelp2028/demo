using Core.Common.Extensions;
using Core.Common.Generic;
using Core.Common.Services;
using StackExchange.Redis;
using System;

namespace Core.Data.Services
{
    /// <summary>登录信息存储</summary>
    public sealed class LoginService : ILoginService
    {
        private readonly IDatabase _database;

        /// <summary>有效期</summary>
        public static TimeSpan Expiry = TimeSpan.FromMinutes(5);

        /// <summary>登录信息存储</summary>
        public LoginService(IRedisContext redisContext)
        {
            _database = redisContext.Database;
        }

        /// <summary>登入</summary>
        /// <param name="userInfo">会员数据</param>
        /// <returns>返回token</returns>
        public string Login(UserInfo userInfo)
        {
            var token = Guid.NewGuid().ToString("N");

            _database.SetObject(RedisKeys.User_Logins_Token + token, userInfo, Expiry);

            return token;
        }

        /// <summary>登出</summary>
        /// <param name="token">token</param>
        public void Logout(string token)
        {
            _database.KeyDelete(RedisKeys.User_Logins_Token + token);
        }

        /// <summary>更新</summary>
        /// <param name="token">token</param>
        /// <param name="userInfo">会员数据</param>
        public void Update(string token, UserInfo userInfo)
        {
            _database.SetObject(RedisKeys.User_Logins_Token + token, userInfo, Expiry);
        }

        /// <summary>获取</summary>
        /// <param name="token">token</param>
        public UserInfo Get(string token)
        {
            return _database.GetObject<UserInfo>(RedisKeys.User_Logins_Token + token);
        }

        /// <summary>重设会员数据的有效期</summary>
        /// <param name="token">token</param>
        public void AddExpire(string token)
        {
            _database.KeyExpire(RedisKeys.User_Logins_Token + token, Expiry);
        }
    }
}