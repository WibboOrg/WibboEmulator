using System;

namespace Butterfly.Core
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
                        Logging.LogMessage("Server exiting at " + DateTime.Now);
                        Logging.DisablePrimaryWriting(true);
                        Console.WriteLine("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                        ButterflyEnvironment.PreformShutDown(true);
                        break;
                    case "forceshutdown":
                        ButterflyEnvironment.GetGame().gameLoopEnded = true;
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        Logging.LogMessage(parameters[0].ToLower() + " is an unknown or unsupported command. Type help for more information");
                        break;
                }
            }
            catch (Exception e)
            {
                Logging.LogMessage("Error in command [" + inputData + "]: " + e);
            }
        }
    }
}