using Core.Common.Generic;
using Core.Common.Services;
using StackExchange.Redis;
using System;

namespace Core.Data.Services
{
    /// <summary>验证码方法一，验证码攻击无效，只对验证码验证时攻击才有效</summary>
    public sealed class CheckCodeService : ICheckCodeService
    {
        private const int EXPIRE = 30;
        private readonly IDatabase _database;
        private readonly ISecretService _secretService;
        private readonly ISysClockService _sysClockService;

        /// <summary>验证码</summary>
        public CheckCodeService(IRedisContext redisContext, ISecretService secretService, ISysClockService sysClockService)
        {
            _database = redisContext.Database;
            _secretService = secretService;
            _sysClockService = sysClockService;
        }

        /// <summary>解密对比验证码和有效期</summary>
        public bool IsValidCode(string codeid, string code)
        {
            if (string.IsNullOrWhiteSpace(codeid))
                return false;

            if (codeid.Length > 32)
                return false;

            if (codeid.Contains(":"))
                return false;

            try
            {
                var str = _secretService.Decrypt(codeid, "|" + code + "|" + code + "|" + code + "|");
                var values = str.Split('@', 2);

                var now = _sysClockService.GetDate();
                var expire = DateTime.ParseExact(values[1], "yyyyMMddHHmmss", null);

                // guid已过期
                if (now > expire)
                    return false;

                // 该guid已使用过
                if (_database.KeyExists(RedisKeys.User_CheckCodes + codeid))
                    return false;

                // 使guid失效
                _database.StringSet(RedisKeys.User_CheckCodes + codeid, code, expire - now);

                // 验证码不正确
                if (values[0] != code)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>返回加密的(验证码和有效期)guid</summary>
        public string SetCheckCode(string code)
        {
            var expire = _sysClockService.GetDate().AddSeconds(EXPIRE);
            var guid = _secretService.Encrypt(code + "@" + expire.ToString("yyyyMMddHHmmss"), "|" + code + "|" + code + "|" + code + "|");
            return guid;
        }
    }

    ///// <summary>验证码方法二，可能会遭验证码攻击</summary>
    //public sealed class CheckCodeService2 : ICheckCodeService
    //{
    //    private const int EXPIRE = 30;
    //    private readonly IDatabase database;
    //    private readonly ICryptoService crypto;
    //    private readonly IDateTimeService dateTime;

    //    /// <summary>验证码</summary>
    //    public CheckCodeService2(IRedisService _redisAccessor, ICryptoService _crypto, IDateTimeService _dateTime)
    //    {
    //        database = _redisAccessor.Database;
    //        crypto = _crypto;
    //        dateTime = _dateTime;
    //    }

    //    /// <summary>比较验证将从redis删除</summary>
    //    public bool IsValid(string guid, string code)
    //    {
    //        var oldCode = (string)database.StringGet(RedisKeys.User_Codes + guid);

    //        if (oldCode == null)
    //            return false;

    //        database.KeyDelete(RedisKeys.User_Codes + guid);
    //        return oldCode == code;
    //    }

    //    /// <summary>将验证码写入redis</summary>
    //    public string SetCode(string code)
    //    {
    //        var guid = Guid.NewGuid().ToString("N");
    //        database.StringSet(RedisKeys.User_Codes + guid, code, TimeSpan.FromSeconds(EXPIRE));
    //        return guid;
    //    }
    //}
}