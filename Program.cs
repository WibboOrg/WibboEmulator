using WibboEmulator.Core;

namespace WibboEmulator
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            WibboEnvironment.Initialize();

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

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            ExceptionLogger.DisablePrimaryWriting(true);
            ExceptionLogger.LogCriticalException("SYSTEM CRITICAL EXCEPTION: " + ((Exception)args.ExceptionObject).ToString());

            WibboEnvironment.PreformShutDown();
        }
    }
}
