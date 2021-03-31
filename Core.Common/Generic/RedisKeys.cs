namespace Core.Common.Generic
{
    /// <summary>RedisKeys 集中管理</summary>
    public class RedisKeys
    {
        /// <summary>登录信息 Token</summary>
        public const string User_Logins_Token = "users:logins:token:";

        /// <summary>验证码</summary>
        public const string User_CheckCodes = "users:codes:";

        /// <summary>流水号</summary>
        public const string User_Sequences_SequenceKey = "users:sequences:seq";
        public const string User_Sequences_IdentityKey = "users:sequences:id";

        /// <summary>锁</summary>
        public const string User_Lock_SequenceKey = "users:lock:sequence";

        ///// <summary>聊天室</summary>
        //public const string Chat_Connection = "chats:connections:";
    }
}