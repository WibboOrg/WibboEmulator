using Butterfly.Core;
using Butterfly.Utilities;
using System;
using System.Security.Permissions;

namespace Butterfly
{
    public static class Program
    {
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MyHandler;

            ButterflyEnvironment.Initialize();

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write("Command> ");
                    string Input = Console.ReadLine();

                    if (Input.Length > 0)
                    {
                        string s = Input.Split(' ')[0];

                        ConsoleCommands.InvokeCommand(s);
                    }
                }
            }
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            ExceptionLogger.DisablePrimaryWriting(true);
            ExceptionLogger.LogCriticalException("SYSTEM CRITICAL EXCEPTION: " + ((Exception)args.ExceptionObject).ToString());

            ButterflyEnvironment.PreformShutDown();
        }
    }
}
