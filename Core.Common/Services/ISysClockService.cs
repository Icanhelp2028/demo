using System;

namespace Core.Common.Services
{
    /// <summary>服务器当前时间</summary>
    public interface ISysClockService
    {
        DateTime GetDate();
    }
}