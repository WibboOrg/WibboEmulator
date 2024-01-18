namespace WibboEmulator.Core;

using WibboEmulator.Utilities;

public static class ExceptionLogger
{
    public static void WriteLine(string line) => LogWriter.WriteLine(line);

    public static void LogException(string logText) => LogWriter.LogException(logText);

    public static void LogCriticalException(string logText) => LogWriter.LogCriticalException(logText);

    public static void LogDenial(string logText) => LogWriter.LogDDOS(logText);

    public static void LogMessage(string logText) => LogWriter.LogMessage(logText);

    public static void LogThreadException(string exception, string threadName) => LogWriter.LogThreadException(exception, threadName);

    public static void LogPacketException(string packet, string exception) => LogWriter.LogPacketException(packet, exception);

    public static void LogWebSocket(string logText) => LogWriter.LogWebsocketException(logText);

    public static void DisablePrimaryWriting(bool clearConsole) => LogWriter.DisablePrimaryWriting(clearConsole);
}
