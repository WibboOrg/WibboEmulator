namespace WibboEmulator.Core;
using System.Text;
using WibboEmulator.Utilities.ConsoleWriter;

public static class ExceptionLogger
{
    public static bool DisabledState
    {
        get => Writer.DisabledState;
        set => Writer.DisabledState = value;
    }

    public static void WriteLine(string line) => Writer.WriteLine(line);

    public static void LogException(string logText) => Writer.LogException(DateTime.Now.ToString() + ": " + Environment.NewLine + logText + Environment.NewLine);

    public static void LogCriticalException(string logText) => Writer.LogCriticalException(DateTime.Now.ToString() + ": " + logText);

    public static void LogCacheError(string logText) => Writer.LogCacheError(DateTime.Now.ToString() + ": " + logText);

    public static void LogDenial(string logText) => Writer.LogDDOSS(DateTime.Now.ToString() + ": " + logText);

    public static void LogMessage(string logText) => Writer.LogMessage(DateTime.Now.ToString() + ": " + logText);

    public static void LogThreadException(string exception, string threadname) => Writer.LogThreadException(DateTime.Now.ToString() + ": " + exception, threadname);

    public static void LogQueryError(Exception exception, string query) => Writer.LogQueryError(exception, DateTime.Now.ToString() + ": " + query);

    public static void LogPacketException(string packet, string exception) => Writer.LogPacketException(packet, DateTime.Now.ToString() + " : " + exception);

    public static void HandleException(Exception pException, string pLocation) => Writer.HandleException(pException, DateTime.Now.ToString() + ": " + pLocation);

    public static void DisablePrimaryWriting(bool clearConsole) => Writer.DisablePrimaryWriting(clearConsole);

    public static void LogShutdown(StringBuilder builder) => Writer.LogShutdown(builder);
}
