namespace IBLL.Injections
{
    public enum ChatNotifyType
    {
        /// <summary>
        /// 一般消息
        /// </summary>
        UserMsg = 1,

        /// <summary>
        /// 会员进入群组
        /// </summary>
        EnterGroup = 2,

        /// <summary>
        /// 会员离开群组
        /// </summary>
        ExitGroup = 3
    }

    public class ChatUserInfo
    {
        /// <summary>会员ID</summary>
        public int UserId { get; set; }

        /// <summary>会员昵称</summary>
        public string NickName { get; set; }

        /// <summary>会员头像</summary>
        public string Logo { get; set; }

        /// <summary>会员心情</summary>
        public string Descr { get; set; }

        /// <summary>会员等级</summary>
        public int Level { get; set; }
    }

    public class ChatMessage
    {
        /// <summary>谁发的信息</summary>
        public int FromUser { get; set; }

        /// <summary>哪个群组接收信息</summary>
        public int ToGroup { get; set; }

        /// <summary>文本内容</summary>
        public string Text { get; set; }

        /// <summary>非文本内容</summary>
        public object Data { get; set; }

        /// <summary>消息类型</summary>
        public ChatNotifyType Notify { get; set; }
    }

    public class ChatGroup
    {
        /// <summary>群组ID</summary>
        public int Id { get; set; }

        /// <summary>群组名称</summary>
        public string Name { get; set; }
    }

    public interface IChatBLL
    {
        /// <summary>新建群组</summary>
        int CreateGroup(string groupName);

        /// <summary>增加会员入群权限</summary>
        void AddGroup(int groupId, int[] userIds);

        /// <summary>移出会员入群权限</summary>
        void RemoveGroup(int groupId, int[] userIds);

        /// <summary>获取所有群组</summary>
        ChatGroup[] GetGroupsFromUserId(int userId);

        /// <summary>获取群组成员</summary>
        ChatUserInfo[] GetUsersInfoFromGroup(int groupId);

        /// <summary>获取群组消息</summary>
        ChatMessage[] GetChatMsg(int groupId);

        /// <summary>获取所有成员，成员->群组->成员</summary>
        int[] GetUsersFromUserId(int userId);

        /// <summary>发送消息到群组</summary>
        void SendMsg(int groupId, ChatMessage chatMessage);

        bool CanAddGroup(int groupId);

        bool CanRemoveGroup(int groupId);

        bool CanEnterGroup(int groupId);

        bool CanSendMsg(int[] groupIds);
    }
}