// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Text;
using UnityEngine;

namespace Sendbird.Calls
{
    internal static class Logger
    {
        internal enum CategoryType
        {
            Common,
            Api,
            WebSocket,
            Command,
            Room,
            Rtc,
        }
        
        private static LogLevel _logLevel = LogLevel.None;
        private static readonly StringBuilder _tempStringBuilder = new StringBuilder();

        internal static void LogError(CategoryType categoryType, string message)
        {
            if (LogLevel.Error <= _logLevel)
            {
                _tempStringBuilder.Clear();
                _tempStringBuilder.Append($"[Sb{categoryType}_{Time.frameCount}]{message}");
                Debug.LogError(_tempStringBuilder);
            }
        }
        
        internal static void LogWarning(CategoryType categoryType, string message)
        {
            if (LogLevel.Warning <= _logLevel)
            {
                _tempStringBuilder.Clear();
                _tempStringBuilder.Append($"[Sb{categoryType}_{Time.frameCount}]{message}");
                Debug.LogWarning(_tempStringBuilder);
            }
        }
        
        internal static void LogInfo(CategoryType categoryType, string message)
        {
            if (LogLevel.Info <= _logLevel)
            {
                _tempStringBuilder.Clear();
                _tempStringBuilder.Append($"[Sb{categoryType}_{Time.frameCount}]{message}");
                Debug.Log(_tempStringBuilder);
            }
        }

        internal static void SendLogsToServer(LogLevel logLevel)
        {
            //disable send to server because experimental
            //_logCollector.Flush(logLevelType);
        }
        
        internal static void SetLogLevel(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }
    }
}