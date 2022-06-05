using Butterfly.Core;

namespace Butterfly
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MyHandler;

            ButterflyEnvironment.Initialize();

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write("Command> ");
                    string Input = Console.ReadLine();

                    if (Input != null && Input.Length > 0)
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
