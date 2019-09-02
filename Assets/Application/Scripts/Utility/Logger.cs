using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using UnityEngine;

public static class Logger
{
    // SPEC:
    //   Assets 配下にファイル出力すると、UnityEditor が新規ファイルと検知して表示しようとするせいか、
    //   動作としては正常なものの、ファイル検知したタイミングで unity に掴まれていますエラーがでてしまう。
    //   そのため出力個所を Assets 配下でないところにしている。
#if UNITY_EDITOR
    private static readonly string LOG_PATH = Path.Combine(@"C:\work\temp\", "Logs");
#else
    private static readonly string LOG_PATH = Path.Combine(Application.temporaryCachePath , "Logs");
#endif
    private static readonly Level ROOT_LEVEL = Level.All;

    private static ILog logger;

    private static bool isInitialized = false;

    public static void Init()
    {
        // SPEC:
        //   初期化は一回しかできない。
        if(isInitialized) return;
        
        logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // SPEC:
        //   DatePattern はファイルローテーションのタイミングでファイル名に適用されることから、
        //   アプリ起動ごとにファイルを切り替えたい仕様に合わず、
        //   ファイル名を初期軌道のタイミングで受け取ることにした。
        var fileName = "application_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log";
        ConfigureAllLogging(fileName);

        logger.Info("Logging is started, log level is " + ROOT_LEVEL + ".");
        logger.Info("Log file is " + Path.Combine(LOG_PATH, fileName) + ".");

        isInitialized = true;
    }

    public static void Debug(string message)
    {
        logger.Debug(message);
    }

    public static void Info(string message)
    {
        logger.Info(message);
    }

    public static void Warn(string message)
    {
        logger.Warn(message);
    }

    public static void Error(string message)
    {
        logger.Error(message);
    }

    public static void Fatal(string message)
    {
        logger.Fatal(message);
    }

    private static void ConfigureAllLogging(string fileName)
    {
        var root = ((Hierarchy) logger.Logger.Repository).Root;
        root.Level = ROOT_LEVEL;

        var patternLayout = new PatternLayout
        {
            // Memo: default %d = yyyy/MM/dd HH:mm:ss,fff
            ConversionPattern = "%d{yyyy/MM/dd HH:mm:ss.fff} %-5level %logger - %message%newline"
        };
        patternLayout.ActivateOptions();

        var fileAppender = new RollingFileAppender
        {
            AppendToFile = false,
            File = Path.Combine(LOG_PATH, fileName),
            Layout = patternLayout,
            RollingStyle = RollingFileAppender.RollingMode.Once,
            StaticLogFileName = true
        };
        fileAppender.ActivateOptions();

        var unityLogger = new UnityAppender
        {
            Layout = new PatternLayout()
        };
        unityLogger.ActivateOptions();

        BasicConfigurator.Configure(unityLogger, fileAppender);
    }

    /// <summary> An appender which logs to the unity console. </summary>
    private class UnityAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var message = RenderLoggingEvent(loggingEvent);

            if (Level.Compare(loggingEvent.Level, Level.Error) >= 0)
            {
                UnityEngine.Debug.LogError(message);
            }
            else if (Level.Compare(loggingEvent.Level, Level.Warn) >= 0)
            {
                UnityEngine.Debug.LogWarning(message);
            }
            else
            {
                UnityEngine.Debug.Log(message);
            }
        }
    }
}