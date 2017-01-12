using LiveTileTestPrism.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace LiveTileTestPrism.Services
{
    public class LoggerService : ILoggerService
    {
        #region Members
        public bool EnableLogQueries { get; set; }
        public bool EnableLogInfo { get; set; }
        public bool EnableLogErrors { get; set; }
        public object SeverityLevel { get; private set; }
        #endregion

        private ConcurrentQueue<string> LogInfoMessages;
        private ConcurrentQueue<string> LogErrorMessages;
        private ConcurrentQueue<string> LogVerboseMessages;

        public LoggerService()
        {
            // Default settings for logger
            EnableLogErrors = true;
            EnableLogInfo = true;
            EnableLogQueries = false;

            LogInfoMessages = new ConcurrentQueue<string>();
            LogErrorMessages = new ConcurrentQueue<string>();
            LogVerboseMessages = new ConcurrentQueue<string>();
        }

        public async Task DumpLogsToFileAsync()
        {
            var infoLines = GetDebugLines(ref LogInfoMessages);
            if (infoLines.Count() > 0)
            {
                var logFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("info.log", CreationCollisionOption.OpenIfExists);
                await FileIO.AppendLinesAsync(logFile, infoLines);
            }

            var errorLines = GetDebugLines(ref LogErrorMessages);
            if (errorLines.Count() > 0)
            {
                var logFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("error.log", CreationCollisionOption.OpenIfExists);
                await FileIO.AppendLinesAsync(logFile, errorLines);
            }

            var verboseLines = GetDebugLines(ref LogVerboseMessages);
            if (verboseLines.Count() > 0)
            {
                var logFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("verbose.log", CreationCollisionOption.OpenIfExists);
                await FileIO.AppendLinesAsync(logFile, verboseLines);
            }
        }

        private List<string> GetDebugLines(ref ConcurrentQueue<string> queue)
        {
            var retVal = new List<string>();

            string e;
            while (queue.TryDequeue(out e))
                retVal.Add(e);

            return retVal;
        }

        public void LogQuery(string sqlQuery, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var debugMessage = $"@@@ SQL Query in '{methodName}' @@@ [{Path.GetFileName(filePath)} ({lineNumber})] -- {sqlQuery}";
#if DEBUG
            if (EnableLogQueries)
                Debug.WriteLine(debugMessage);
#else
            LogVerboseMessages.Enqueue($"{DateTime.Now.ToString()} | {debugMessage}");
#endif

        }

        public void LogError(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var debugMessage = $"!!! Error in '{methodName}' !!! [{Path.GetFileName(filePath)} ({lineNumber})] -- {message}";
#if DEBUG
            if (EnableLogErrors)
                Debug.WriteLine(debugMessage);
#else
            LogErrorMessages.Enqueue($"{DateTime.Now.ToString()} | {debugMessage}");
#endif
        }

        public void LogInfo(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var debugMessage = $"<<< '{methodName}' >>> [{Path.GetFileName(filePath)} ({lineNumber})] -- {message}";
#if DEBUG
            if (EnableLogInfo)
                Debug.WriteLine(debugMessage);
#else
            LogInfoMessages.Enqueue($"{DateTime.Now.ToString()} | {debugMessage}");
#endif
        }
    }
}
