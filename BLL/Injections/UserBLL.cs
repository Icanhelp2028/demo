using BLL.Db;
using IBLL.Injections;
using IBLL.Models.UserDb;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Injections
{
    public class UserBLL : IUserBLL
    {
        private readonly UserDb _userDb;

        public UserBLL(UserDb userDb)
        {
            _userDb = userDb;
        }

        public void AddUser(UserMdl userModel)
        {
            _userDb.Users.Add(userModel);
            _userDb.SaveChanges();
        }

        public IList<UserMdl> GetList()
        {
            return _userDb.Users.ToList();
        }
    }
}