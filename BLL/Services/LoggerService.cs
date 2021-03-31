using IBLL.Injections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Transactions;

namespace BLL.Services
{
    /// <summary>记录日志</summary>
    public sealed class LoggerService : ILogger
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public LoggerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new TransactionScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var _logBLL = (ILogBLL)serviceScope.ServiceProvider.GetService(typeof(ILogBLL));
            var message = formatter(state, exception);

            _logBLL.Add(eventId.Id, eventId.Name, (int)logLevel, message);
        }
    }
}