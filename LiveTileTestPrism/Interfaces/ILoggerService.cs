using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LiveTileTestPrism.Interfaces
{
    public interface ILoggerService
    {
        bool EnableLogQueries { get; set; }
        bool EnableLogInfo { get; set; }
        bool EnableLogErrors { get; set; }

        void LogQuery(string sqlQuery, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0);
        void LogError(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0);
        void LogInfo(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0);

        Task DumpLogsToFileAsync();
    }
}
