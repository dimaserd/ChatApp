using System;
using System.Threading.Tasks;

namespace CrocoChat.Simple.Abstractions
{
    public interface ILoggerManager
    {
        Task LogExceptionAsync(Exception ex);
    }
}