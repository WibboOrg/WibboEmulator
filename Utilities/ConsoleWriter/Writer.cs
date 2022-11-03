namespace WibboEmulator.Utilities.ConsoleWriter;
using System.Collections;
using System.Text;
using WibboEmulator;

public class Writer
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

    public static void LogException(string logText)
    {
        WriteToFile("logs/exceptions.txt", logText + "\r\n\r\n");
        WriteLine("Exception has been saved");
    }

    public static void LogCriticalException(string logText)
    {
        WriteToFile("logs/criticalexceptions.txt", logText + "\r\n\r\n");
        WriteLine("CRITICAL ERROR LOGGED");
    }

    public static void LogMessage(string logText) => Console.WriteLine(logText);

    public static void LogDDOS(string logText)
    {
        WriteToFile("logs/ddos.txt", logText + "\r\n\r\n");
        WriteLine(logText);
    }

    public static void LogThreadException(string exception, string threadName)
    {
        WriteToFile("logs/threaderror.txt", "Error in thread " + threadName + ": \r\n" + exception + "\r\n\r\n");
        WriteLine("Error in " + threadName + " caught");
    }

    public static void LogQueryError(Exception exception, string query)
    {
        WriteToFile("logs/MySQLerrors.txt", "Error in query: \r\n" + (object)query + "\r\n" + exception.ToString() + "\r\n\r\n");
        WriteLine("Error in query caught");
    }

    public static void LogPacketException(string packet, string exception)
    {
        WriteToFile("logs/packeterror.txt", "Error in packet " + packet + ": \r\n" + exception + "\r\n\r\n");
        WriteLine("User disconnection logged: " + exception);
    }

    public static void HandleException(Exception exception, string location)
    {
        var stringBuilder = new StringBuilder();
        _ = stringBuilder.AppendLine("Exception logged " + DateTime.Now.ToString() + " in " + location + ":");
        _ = stringBuilder.AppendLine(exception.ToString());
        if (exception.InnerException != null)
        {
            _ = stringBuilder.AppendLine("Inner exception:");
            _ = stringBuilder.AppendLine(exception.InnerException.ToString());
        }
        if (exception.HelpLink != null)
        {
            _ = stringBuilder.AppendLine("Help link:");
            _ = stringBuilder.AppendLine(exception.HelpLink);
        }
        if (exception.Source != null)
        {
            _ = stringBuilder.AppendLine("Source:");
            _ = stringBuilder.AppendLine(exception.Source);
        }
        if (exception.Data != null)
        {
            _ = stringBuilder.AppendLine("Data:");
            foreach (DictionaryEntry dictionaryEntry in exception.Data)
            {
                _ = stringBuilder.AppendLine("  Key: " + dictionaryEntry.Key + "Value: " + dictionaryEntry.Value);
            }
        }
        if (exception.Message != null)
        {
            _ = stringBuilder.AppendLine("Message:");
            _ = stringBuilder.AppendLine(exception.Message);
        }
        if (exception.StackTrace != null)
        {
            _ = stringBuilder.AppendLine("Stack trace:");
            _ = stringBuilder.AppendLine(exception.StackTrace);
        }
        _ = stringBuilder.AppendLine();
        _ = stringBuilder.AppendLine();
        LogException(stringBuilder.ToString());
    }

    public static void DisablePrimaryWriting(bool clearConsole)
    {
        DisabledState = true;
        if (!clearConsole)
        {
            return;
        }

        Console.Clear();
    }

    private static void WriteToFile(string path, string content)
    {
        try
        {
            var fullPath = WibboEnvironment.PatchDir + path;
            var fullPathDir = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(fullPathDir))
            {
                _ = Directory.CreateDirectory(fullPathDir);
            }

            using var writer = new StreamWriter(fullPath, true);
            using var syncWriter = TextWriter.Synchronized(writer);
            syncWriter.WriteLine(content);
        }
        catch (Exception ex)
        {
            WriteLine("Could not write to file: " + ex.ToString() + ":" + content);
        }
    }
}
