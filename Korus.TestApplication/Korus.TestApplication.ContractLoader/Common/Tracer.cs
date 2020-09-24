using System;
using System.IO;
using System.Text;


namespace Korus.TestApplication.ContractLoader.Common
{
    public sealed class Tracer
    {

        private const string LogsDirectorypath = "Logs";

        private const string TraceFormat = "yyyy.MM.dd | HH:mm:ss";
        private const string Separator = "  |  ";


        static Tracer()
        {
            if (!Directory.Exists(LogsDirectorypath))
            {
                Directory.CreateDirectory(LogsDirectorypath);
            }
        }


        public static void Trace(string message)
        {
            WriteLine(FormatMessage(message));
        }


        private static void WriteLine(string message)
        {

            if (!string.IsNullOrEmpty(message))
            {
                var writer = GetFile();
                if (writer != null)
                {
                    writer.WriteLine(message);
                    writer.Close();
                }
            }
        }

        private static string FormatMessage(string message)
        {
            var now = DateTime.Now;
            var stampStr = now.ToString(TraceFormat);
            var stringBuilder = new StringBuilder(256);

            stringBuilder.Append(stampStr);
            stringBuilder.Append(Separator);

            if (!string.IsNullOrEmpty(message))
            {
                stringBuilder.Append(message);
            }

            stringBuilder.Replace('\0', ' ');

            return stringBuilder.ToString();
        }

        private static StreamWriter GetFile()
        {
            StreamWriter writer;

            var dtNow = DateTime.Now;

            string logFileName = $"Log_{dtNow.Year}_{dtNow.Month}_{dtNow.Day}.txt";

            string logFile = Path.Combine(LogsDirectorypath, logFileName);

            if (File.Exists(logFile))
            {
                writer = new StreamWriter(logFile, true);
            }
            else
            {
                var fs = new FileStream(logFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                writer = new StreamWriter(fs);
            }
            return writer;
        }
    }

}
