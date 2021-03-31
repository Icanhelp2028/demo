using BLL.Db;
using IBLL.Injections;
using IBLL.Models.UserDb;

namespace BLL.Injections
{
    public class LogBLL : ILogBLL
    {
        private readonly UserDb _userDb;
        public LogBLL(UserDb userDb)
        {
            _userDb = userDb;
        }

        public void Add(int id, string name, int logLevel, string message)
        {
            _userDb.Logs.Add(new LogMdl
            {
                SourceId = id,
                SourceName = name,
                LogLevel = logLevel,
                Body = message
            });
            _userDb.SaveChanges();
        }
    }
}