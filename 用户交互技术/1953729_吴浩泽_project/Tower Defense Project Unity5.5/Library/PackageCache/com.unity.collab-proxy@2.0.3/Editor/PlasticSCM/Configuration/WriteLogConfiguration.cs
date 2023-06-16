using System.IO;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class WriteLogConfiguration
    {
        internal static void For(string logConfigPath)
        {
            string logDirectoryPath = GetPlasticLogDirectoryPath(logConfigPath);
            string relevantLogFile = Path.Combine(logDirectoryPath, RELEVANT_LOG_FILE_NAME);
            string debugLogFile = Path.Combine(logDirectoryPath, DEBUG_LOG_FILE_NAME);

            using (StreamWriter sw = File.CreateText(logConfigPath))
            {
                sw.Write(string.Format(
                    LOG_CONFIGURATION,
                    relevantLogFile,
                    debugLogFile));
            }
        }

        static string GetPlasticLogDirectoryPath(string logConfigPath)
        {
            return Path.Combine(
                Directory.GetParent(logConfigPath).FullName,
                LOGS_DIRECTORY);
        }

        const string LOGS_DIRECTORY = "logs";
        const string RELEVANT_LOG_FILE_NAME = "unityplastic.relevant.log.txt";
        const string DEBUG_LOG_FILE_NAME = "unityplastic.debug.log.txt";
        const string LOG_CONFIGURATION = 
@"<log4net>
  <appender name=""RelevantInfoAppender"" type=""log4net.Appender.RollingFileAppender"">
    <file value=""{0}"" />
    <appendToFile value=""true"" />
    <rollingStyle value=""Size"" />
    <maxSizeRollBackups value=""10"" />
    <maximumFileSize value=""2MB"" />
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value=""%date %username %-5level %logger - %message%newline"" />
    </layout>
    <filter type=""log4net.Filter.LevelRangeFilter""><levelMin value=""INFO"" /><levelMax value=""FATAL"" /></filter>
  </appender>

  <appender name=""DebugAppender"" type=""log4net.Appender.RollingFileAppender"">
    <file value=""{1}"" />
    <appendToFile value=""true"" />
    <rollingStyle value=""Size"" />
    <maxSizeRollBackups value=""10"" />
    <maximumFileSize value=""10MB"" />
    <staticLogFileName value=""true"" />
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value=""%date %username %-5level %logger - %message%newline"" />
    </layout>
  </appender>

  <root>
    <level value=""DEBUG"" />
    <appender-ref ref=""RelevantInfoAppender"" />
    <appender-ref ref=""DebugAppender"" />
  </root>
</log4net>
";
    }
}
