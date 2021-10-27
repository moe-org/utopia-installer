using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaInstaller
{
    /// <summary>
    /// 日志设置器
    /// </summary>
    internal static class LogManager
    {

        private static readonly object _lock = new();

        private static bool _setted = false;

        /// <summary>
        /// 设置日志器
        /// </summary>
        public static void EnableConfig()
        {
            lock (_lock)
            {
                if (_setted)
                    return;

                _setted = true;

                var config = new NLog.Config.LoggingConfiguration();

                var logfile = new NLog.Targets.FileTarget("logfile") { 
                    FileName = "utopia-installer.log",
                    Layout = "[${longdate}] [${level}] [t-${threadid}] [${logger}]:${message} ${exception}",
                    Encoding = Encoding.UTF8,
                    LineEnding = NLog.Targets.LineEndingMode.LF,
                    EnableFileDelete = true
                };
         
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
 
                NLog.LogManager.Configuration = config;
            }
        }







    }
}
