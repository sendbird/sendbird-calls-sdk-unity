// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal static class LogLevelExtension
    {
        private static string ToJsonPropertyName(this LogLevel logLevel)
        {
            return NewtonsoftJsonExtension.EnumToJsonPropertyName(logLevel);
        }

        internal static LogLevel JsonPropertyNameToType(string jsonPropertyName)
        {
            for (LogLevel logLevel = LogLevel.Verbose; logLevel <= LogLevel.None; logLevel++)
                if (logLevel.ToJsonPropertyName().Equals(jsonPropertyName))
                    return logLevel;

            return LogLevel.None;
        }
        
        internal static LogLevel ToLogLevelType(this SbLogLevel sbLogLevel)
        {
            switch (sbLogLevel)
            {
                case SbLogLevel.Info:    return LogLevel.Info;
                case SbLogLevel.Warning: return LogLevel.Warning;
                case SbLogLevel.Error:   return LogLevel.Error;
                case SbLogLevel.None:    return LogLevel.None;
            }

            return LogLevel.None;
        }
    }
}