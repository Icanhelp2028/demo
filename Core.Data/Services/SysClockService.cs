using Core.Common.Services;
using System;
using System.Data.SqlClient;

namespace Core.Data.Services
{
    /// <summary>服务器当前时间</summary>
    public sealed class SysClockService : ISysClockService
    {
        private SqlCommand _command;
        private readonly string _connectionString;

        /// <summary>服务器当前时间</summary>
        public SysClockService(string connectionString)
        {
            _connectionString = connectionString;
            _command = CreateSqlCommand();
        }

        private SqlCommand CreateSqlCommand()
        {
            // 使用长连接
            var sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            return new SqlCommand("select getdate()", sqlConnection);
        }

        public DateTime GetDate()
        {
            try
            {
                return (DateTime)_command.ExecuteScalar();
            }
            catch
            {
                _command = CreateSqlCommand();
                return (DateTime)_command.ExecuteScalar();
            }
        }
    }
}