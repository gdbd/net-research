using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace Korus.TestApplication.Common
{
    namespace TestApplication.Common
    {
        public static class Logger
        {
            public static string DefaultAreaName = "TestApplication_Trace";

            private static Dictionary<string, LoggerImpl> _loggers = new Dictionary<string, LoggerImpl>();
            private static LoggerImpl _logger(string area = "")
            {
                if (_loggers.ContainsKey(area))
                {
                    return _loggers[area];
                }

                var logger = LoggerImpl.GetInstance(area);
                _loggers.Add(area, logger);
                return logger;
            }

            public static void Write(string message, params object[] args)
            {
                _logger().Write(message, args);
            }

            public static void WriteTo(string message, string area, params object[] args)
            {
                _logger(area).Write(message, args);
            }

            public static void Error(string message, params object[] args)
            {
                _logger().Error(message, args);
            }

            public static void Exception(Exception exception, params object[] args)
            {
                _logger().Exception(exception);
            }

            class LoggerImpl : SPDiagnosticsServiceBase
            {
                private string DiagnosticAreaName = Logger.DefaultAreaName;

                internal static LoggerImpl GetInstance(string area)
                {
                    return new LoggerImpl(area);
                }

                private LoggerImpl(string area) : base("Logging Service", SPFarm.Local)
                {
                    if (area.Length != 0) DiagnosticAreaName = area;
                }

                protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
                {
                    var areas = new List<SPDiagnosticsArea>
                {
                    new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
                    {
                        new SPDiagnosticsCategory("Exception", TraceSeverity.Unexpected, EventSeverity.ErrorCritical),
                        new SPDiagnosticsCategory("Error", TraceSeverity.High, EventSeverity.Error),
                        new SPDiagnosticsCategory("Trace", TraceSeverity.Medium, EventSeverity.Information)
                    })
                };

                    return areas;
                }

                public void Write(string message, params object[] args)
                {
                    if (Environment.UserInteractive)
                    {
                        Console.WriteLine(message, args);
                    }

                    var category = Areas[DiagnosticAreaName].Categories["Trace"];
                    WriteTrace(0, category, category.TraceSeverity, message, args);
                }

                public void Error(string message, params object[] args)
                {
                    var category = Areas[DiagnosticAreaName].Categories["Error"];
                    WriteTrace(0, category, category.TraceSeverity, message, args);
                }

                public void Exception(Exception exception)
                {
                    var category = Areas[DiagnosticAreaName].Categories["Trace"];
                    WriteTrace(0, category, category.TraceSeverity, exception.ToString());
                }
            }

        }
    }

}
