using Core.Common.Services;
using Microsoft.AspNetCore.SignalR;

namespace WebChat.Services
{
    public class ChatUserIdProvider : IUserIdProvider
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public ChatUserIdProvider(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var userInfo = _userContextAccessor.UserContext.UserInfo;
            if (userInfo == null)
                return null;

            return userInfo.UserId.ToString();
        }
    }
}