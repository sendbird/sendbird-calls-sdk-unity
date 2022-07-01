// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// Logger Level enumeration.
    /// Log will not be exposed if the priority value is lower than the configured log level. Logger Level follows the following priority.
    /// </summary>
    public enum SbLogLevel
    {
        /// <summary>
        /// Informational messages that are general to the application.
        /// </summary>
        Info,
        /// <summary>
        /// Potentially problematic situation that may indicate potential problems.
        /// </summary>
        Warning,
        /// <summary>
        /// Error events that may represent the failure of normal program execution.
        /// </summary>
        Error,
        /// <summary>
        /// No logging.
        /// </summary>
        None
    }
}