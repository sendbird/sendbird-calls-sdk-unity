// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sendbird.Calls
{
    internal class LogCollector
    {
        private readonly Queue<LogItemCommandObject> _logItems = new Queue<LogItemCommandObject>();

        private const int MAX_LOG_COUNT = 500;

        private bool _isOverflowLogs = false;

        internal void AddLog(LogLevel logLevel, string message)
        {
            if (MAX_LOG_COUNT <= _logItems.Count)
            {
                _isOverflowLogs = true;
                _logItems.Dequeue();
            }

            LogItemCommandObject logItem = new LogItemCommandObject(DateTime.UtcNow.Ticks, logLevel, message);
            _logItems.Enqueue(logItem);
        }

        internal void Flush(LogLevel logLevel)
        {
            IEnumerable<LogItemCommandObject> filteredLogItems = _logItems.Where(logItem => logLevel <= logItem.logLevel);
            LogItemCommandObject[] filteredLogItemsArray = filteredLogItems.ToArray();
            if (0 < filteredLogItemsArray.Length)
            {
                UploadLogsApiCommand.Request request = new UploadLogsApiCommand.Request(filteredLogItemsArray, _isOverflowLogs);
                CommandRouter.Instance.Send(request);
            }
        }
    }
}