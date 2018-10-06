/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System;
using log4net;
using System.Runtime.CompilerServices;

namespace Nirge.Core
{
    public enum eLogPattern
    {
        None,
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
    }

    public static class CLogUtils
    {
        public static void WriteLine(this ILog log, eLogPattern pattern, object message = null, Exception exception = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (log == null)
                return;

            var s = $"{filePath}:{lineNumber}:{memberName} {message}";

            switch (pattern)
            {
                case eLogPattern.Debug:
                    if (exception == null)
                        log.Debug(s);
                    else
                        log.Debug(s, exception);
                    break;
                case eLogPattern.Info:
                    if (exception == null)
                        log.Info(s);
                    else
                        log.Info(s, exception);
                    break;
                case eLogPattern.Warn:
                    if (exception == null)
                        log.Warn(s);
                    else
                        log.Warn(s, exception);
                    break;
                case eLogPattern.Error:
                    if (exception == null)
                        log.Error(s);
                    else
                        log.Error(s, exception);
                    break;
                case eLogPattern.Fatal:
                    if (exception == null)
                        log.Fatal(s);
                    else
                        log.Fatal(s, exception);
                    break;
            }
        }
    }
}
