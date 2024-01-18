namespace WibboEmulator.Utilities;
using WibboEmulator;

public class LogWriter
{
    public static bool DisabledState { get; set; }

    public static void WriteLine(string line)
    {
        if (DisabledState)
        {
            return;
        }

        Console.WriteLine(line);
    }

    public static void LogException(string logText) => WriteToFile("Exception", logText);

    public static void LogCriticalException(string logText) => WriteToFile("Critical", logText);

    public static void LogMessage(string logText) => Console.WriteLine($"{DateTime.Now} [Log] {logText}");

    public static void LogDDOS(string logText) => WriteToFile("Ddos", logText);

    public static void LogThreadException(string exception, string threadName) => WriteToFile("Thread", "Error in thread " + threadName + ": " + exception);

    public static void LogPacketException(string packet, string exception) => WriteToFile("Packet", "Error in packet " + packet + ": " + exception);

    public static void DisablePrimaryWriting(bool clearConsole)
    {
        DisabledState = true;
        if (!clearConsole)
        {
            return;
        }

        Console.Clear();
    }

    private static void WriteToFile(string logName, string logMessage)
    {
        try
        {
            var logFilePath = $"{WibboEnvironment.PatchDir}/logs/{logName}.log";
            var fullPathDir = Path.GetDirectoryName(logFilePath);

            if (!string.IsNullOrEmpty(fullPathDir))
            {
                _ = Directory.CreateDirectory(fullPathDir);
            }

            File.AppendAllText(logFilePath, $"{DateTime.Now} [{logName}] {logMessage}" + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            WriteLine($"{DateTime.Now} [{logName}] logs have been logged");
        }
        catch (Exception ex)
        {
            WriteLine("Could not write to file: " + ex.ToString() + ":" + logMessage);
        }
    }
}
