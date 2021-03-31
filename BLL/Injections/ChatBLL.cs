using BLL.Db;
using Core.Common.Services;
using IBLL.Injections;
using IBLL.Models.ChatDb;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BLL.Injections
{
    public class ChatBLL : IChatBLL
    {
        private readonly UserInfo _userInfo;
        private readonly ChatDb _chatDb;

        public ChatBLL(ChatDb chatDb, IUserContextAccessor userContextAccessor)
        {
            _userInfo = userContextAccessor.UserContext.UserInfo;
            _chatDb = chatDb;
        }

        public int CreateGroup(string groupName)
        {
            var chatGroup = new ChatGroupMdl
            {
                Name = groupName,
                OwnId = _userInfo.OwnId
            };

            _chatDb.ChatGroups.Add(chatGroup);
            _chatDb.SaveChanges();

            return chatGroup.Id;
        }

        public void AddGroup(int groupId, int[] userIds)
        {
            foreach (var userId in userIds)
            {
                var chatGroupUser = new ChatGroupUserMdl
                {
                    GroupId = groupId,
                    UserId = userId,
                    OwnId = _userInfo.OwnId
                };

                _chatDb.ChatGroupUsers.Add(chatGroupUser);
            }

            _chatDb.SaveChanges();
        }

        public void RemoveGroup(int groupId, int[] userIds)
        {
            foreach (var userId in userIds)
            {
                var chatGroupUser = new ChatGroupUserMdl
                {
                    GroupId = groupId,
                    UserId = userId,
                    OwnId = _userInfo.OwnId
                };

                _chatDb.Entry(chatGroupUser).State = EntityState.Deleted;
            }

            _chatDb.SaveChanges();
        }

        public ChatGroup[] GetGroupsFromUserId(int userId)
        {
            var linq = from chatGroup in _chatDb.ChatGroups
                       join chatGroupUser in _chatDb.ChatGroupUsers
                       on chatGroup.Id equals chatGroupUser.GroupId
                       where
                           chatGroup.OwnId == _userInfo.OwnId &&
                           chatGroupUser.UserId == userId
                       select new ChatGroup { Id = chatGroup.Id, Name = chatGroup.Name };

            return linq.ToArray();
        }

        public int[] GetUsersFromUserId(int userId)
        {
            var linq = (
                            from chatGroup in _chatDb.ChatGroupUsers
                            where
                                chatGroup.OwnId == _userInfo.OwnId &&
                                chatGroup.UserId == userId
                            select chatGroup.UserId
                        ).
                       Distinct();

            return linq.ToArray();
        }

        public ChatUserInfo[] GetUsersInfoFromGroup(int groupId)
        {
            var linq = (
                            from chatGroup in _chatDb.ChatGroupUsers
                            join user in _chatDb.Users on chatGroup.UserId equals user.Id
                            where
                                chatGroup.OwnId == _userInfo.OwnId &&
                                chatGroup.GroupId == groupId
                            select new ChatUserInfo { UserId = user.Id, NickName = user.NickName }
                       ).
                       Distinct();

            return linq.ToArray();
        }

        public ChatMessage[] GetChatMsg(int groupId)
        {
            var linq = (
                            from chatMessage in _chatDb.ChatMessages
                            where
                                 chatMessage.OwnId == _userInfo.OwnId &&
                                 chatMessage.GroupId == groupId
                            select new ChatMessage
                            {
                                ToGroup = chatMessage.GroupId,
                                FromUser = chatMessage.FromUser,
                                Text = chatMessage.Text,
                                Notify = chatMessage.Notify,
                                Data = chatMessage.Data
                            }
                       ).
                       ToArray();

            return linq.ToArray();
        }

        public void SendMsg(int groupId, ChatMessage chatMessage)
        {
            _chatDb.ChatMessages.Add(new ChatMessageMdl
            {
                GroupId = chatMessage.ToGroup,
                FromUser = chatMessage.FromUser,
                Data = System.Text.Json.JsonSerializer.Serialize(chatMessage.Data),
                Notify = chatMessage.Notify,
                OwnId = _userInfo.OwnId,
                Text = chatMessage.Text
            });

            _chatDb.SaveChanges();
        }

        public bool CanAddGroup(int groupId)
        {
            return true;
        }

        public bool CanEnterGroup(int groupId)
        {
            return true;
        }

        public bool CanRemoveGroup(int groupId)
        {
            return true;
        }

        public bool CanSendMsg(int[] groupIds)
        {
            return true;
        }
    }
}