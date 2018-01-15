using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    public static class LogWriter
    {
        public static bool Enabled { get; set; } = true;

        public static string LoggingDirectoryPath { get; set; } = Directory.GetCurrentDirectory();

        public static string LoggingFileName { get; set; } = "log";

        public static LogFileExtension LoggingLogFileExt { get; set; } = LogFileExtension.LOG;

        private static string FullPath => $"{LoggingDirectoryPath}/{LoggingFileName}.{LoggingLogFileExt}";

        public static void Log(string toLog)
        {
            if (!Enabled)
                return;

            string message = $"[{DateTime.Now.ToString()}] {AppDomain.CurrentDomain.FriendlyName} : {toLog}\n\n";
            using (StreamWriter sw = new StreamWriter(FullPath, true))
            {
                sw.WriteLine(message);
            }
        }
    }
}
