using UnityEngine;

public static class Logger
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public static bool IsLoggingEnabled = true; // Enable or disable logging globally
    public static LogLevel MinimumLogLevel = LogLevel.Info; // Set the minimum log level

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        if (!IsLoggingEnabled || level < MinimumLogLevel) return;

        switch (level)
        {
            case LogLevel.Info:
                Debug.Log($"[INFO] {message}");
                break;
            case LogLevel.Warning:
                Debug.LogWarning($"[WARNING] {message}");
                break;
            case LogLevel.Error:
                Debug.LogError($"[ERROR] {message}");
                break;
        }
    }

    public static void Info(string message)
    {
        Log(message, LogLevel.Info);
    }

    public static void Warning(string message)
    {
        Log(message, LogLevel.Warning);
    }

    public static void Error(string message)
    {
        Log(message, LogLevel.Error);
    }
}
