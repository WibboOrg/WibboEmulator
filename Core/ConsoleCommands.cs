namespace WibboEmulator.Core;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;

public class ConsoleCommands
{
    public static void InvokeCommand(string inputData)
    {
        if (string.IsNullOrEmpty(inputData))
        {
            return;
        }

        try
        {
            var parameters = inputData.Split(' ');

            switch (parameters[0].ToLower())
            {
                case "stop":
                case "shutdown":
                {
                    ExceptionLogger.LogMessage("Server exiting at " + DateTime.Now);
                    ExceptionLogger.DisablePrimaryWriting(true);
                    WibboEnvironment.PerformShutDown();
                    break;
                }

                case "clear":
                {
                    Console.Clear();
                    break;
                }

                case "alert":
                {
                    var notice = inputData[6..];

                    GameClientManager.SendMessage(new BroadcastMessageAlertComposer(notice));

                    Console.WriteLine("Alert successfully sent.");
                    break;
                }

                default:
                    ExceptionLogger.LogMessage(parameters[0].ToLower() + " is an unknown or unsupported command. Type help for more information");
                    break;
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogMessage("Error in command [" + inputData + "]: " + ex);
        }
    }
}
