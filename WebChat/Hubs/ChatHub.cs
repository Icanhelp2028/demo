using Core.Common.Services;
using IBLL.Injections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly IChatBLL _chatBLL;
        private readonly string _token;
        private readonly UserInfo _userInfo;
        private readonly ILoginService _loginService;

        public ChatHub(IUserContextAccessor userContextAccessor, ILoginService loginService, IChatBLL chatBLL)
        {
            var ctx = userContextAccessor.UserContext;
            _token = ctx.Token;
            _userInfo = ctx.UserInfo;
            _loginService = loginService;
            _chatBLL = chatBLL;
        }

        /// <summary>新建群组</summary>
        public ChatResult CreateGroup(string groupName)
        {
            var groupId = _chatBLL.CreateGroup(groupName);

            return ChatResult.Success(new { groupId });
        }

        /// <summary>增加会员入群权限</summary>
        public ChatResult AddGroup(int groupId, int[] userIds)
        {
            if (!_chatBLL.CanAddGroup(groupId))
                return ChatResult.FailResult;

            _chatBLL.AddGroup(groupId, userIds);

            return ChatResult.SuccessResult;
        }

        /// <summary>移出会员入群权限</summary>
        public ChatResult RemoveGroup(int groupId, int[] userIds)
        {
            if (!_chatBLL.CanRemoveGroup(groupId))
                return ChatResult.FailResult;

            _chatBLL.RemoveGroup(groupId, userIds);

            return ChatResult.SuccessResult;
        }

        /// <summary>会员进入群组</summary>
        public ChatResult EnterGroup(int groupId)
        {
            if (!_chatBLL.CanEnterGroup(groupId))
                return ChatResult.FailResult;

            // 会员信息列表
            var users = _chatBLL.GetUsersInfoFromGroup(groupId);
            var userId = _userInfo.UserId;

            // 推送个人信息
            var user = users.FirstOrDefault(p => p.UserId == userId);
            SendToGroup(groupId, ChatNotifyType.EnterGroup, null, user);

            // 聊天历史
            var msgs = _chatBLL.GetChatMsg(groupId);

            return ChatResult.Success(new
            {
                users,
                msgs
            });
        }

        /// <summary>会员离开群组</summary>
        public ChatResult ExitGroup(int groupId)
        {
            SendToGroup(groupId, ChatNotifyType.ExitGroup);

            return ChatResult.SuccessResult;
        }

        /// <summary>获取所有群组</summary>
        public ChatResult GetGroups()
        {
            var groups = _chatBLL.GetGroupsFromUserId(_userInfo.UserId);
            return ChatResult.Success(groups);
        }

        /// <summary>更新会员在线状态</summary>
        public void Online()
        {
            _loginService.AddExpire(_token);
        }

        /// <summary>发送群组信息</summary>
        public ChatResult SendToGroups(int[] groupIds, string msg)
        {
            if (!_chatBLL.CanSendMsg(groupIds))
                return ChatResult.FailResult;

            foreach (var groupId in groupIds)
            {
                SendToGroup(groupId, ChatNotifyType.UserMsg, msg);
            }

            return ChatResult.SuccessResult;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            SendToGroup(0, ChatNotifyType.ExitGroup);
            return base.OnDisconnectedAsync(exception);
        }

        private static string[] ToUserIdsStr(int[] userIds)
        {
            var userIdsStr = new string[userIds.Length];
            for (int i = 0, c = userIds.Length; i < c; i++)
            {
                userIdsStr[i] = userIds[i].ToString();
            }

            return userIdsStr;
        }

        private void SendToGroup(int groupId, ChatNotifyType notifyType, string text = null, object data = null)
        {
            var chatMessage = new ChatMessage
            {
                FromUser = _userInfo.UserId,
                ToGroup = groupId,
                Notify = notifyType,
                Text = text,
                Data = data
            };

            var userIds = groupId == 0 ?
                _chatBLL.GetUsersFromUserId(_userInfo.UserId).ToArray() :
                _chatBLL.GetUsersInfoFromGroup(groupId).Select(p => p.UserId).ToArray();

            var userIdsStr = ToUserIdsStr(userIds);
            Clients.Users(userIdsStr).Send(chatMessage);
            _chatBLL.SendMsg(groupId, chatMessage);
        }
    }

    public interface IChatHub
    {
        Task Send(ChatMessage message);
    }

    public class ChatResult
    {
        private ChatResult() { }

        public static ChatResult FailResult = new ChatResult { Code = 0 };
        public static ChatResult SuccessResult = new ChatResult { Code = 1 };

        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }

        public static ChatResult Fail(object data, string msg = null)
        {
            return new ChatResult() { Data = data, Msg = msg, Code = 0 };
        }

        public static ChatResult Success(object data, string msg = null)
        {
            return new ChatResult() { Data = data, Msg = msg, Code = 1 };
        }
    }
}