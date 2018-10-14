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
        public static void WriteLine(this ILog log, eLogPattern pattern, object message = null, Exception ex = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (log == null)
                return;

            var s = $"{filePath}:{lineNumber}:{memberName} {message}";

            switch (pattern)
            {
                case eLogPattern.Debug:
                    if (ex == null)
                        log.Debug(s);
                    else
                        log.Debug(s, ex);
                    break;
                case eLogPattern.Info:
                    if (ex == null)
                        log.Info(s);
                    else
                        log.Info(s, ex);
                    break;
                case eLogPattern.Warn:
                    if (ex == null)
                        log.Warn(s);
                    else
                        log.Warn(s, ex);
                    break;
                case eLogPattern.Error:
                    if (ex == null)
                        log.Error(s);
                    else
                        log.Error(s, ex);
                    break;
                case eLogPattern.Fatal:
                    if (ex == null)
                        log.Fatal(s);
                    else
                        log.Fatal(s, ex);
                    break;
            }
        }
    }
}
