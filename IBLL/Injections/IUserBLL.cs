using IBLL.Models.UserDb;
using System.Collections.Generic;

/// <summary>注入BLL接口</summary>
namespace IBLL.Injections
{
    public interface IUserBLL
    {
        void AddUser(UserMdl userModel);

        IList<UserMdl> GetList();
    }
}