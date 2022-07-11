using WibboEmulator.Communication.Packets.Outgoing.Moderation;

namespace WibboEmulator.Core
{
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
                string[] parameters = inputData.Split(' ');

                switch (parameters[0].ToLower())
                {
                    case "stop":
                    case "shutdown":
                        {
                            ExceptionLogger.LogMessage("Server exiting at " + DateTime.Now);
                            ExceptionLogger.DisablePrimaryWriting(true);
                            Console.WriteLine("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                            WibboEnvironment.PreformShutDown();
                            break;
                        }

                    case "clear":
                        {
                            Console.Clear();
                            break;
                        }

                    case "alert":
                        {
                            string notice = inputData.Substring(6);

                            WibboEnvironment.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(notice));

                            Console.WriteLine("Alert successfully sent.");
                            break;
                        }


                    default:
                        ExceptionLogger.LogMessage(parameters[0].ToLower() + " is an unknown or unsupported command. Type help for more information");
                        break;
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogMessage("Error in command [" + inputData + "]: " + e);
            }
        }
    }
}