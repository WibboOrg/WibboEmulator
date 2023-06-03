namespace WibboEmulator;

using WibboEmulator.Core;

public static class Program
{
    public static void Main()
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
                var input = Console.ReadLine();

                if (input != null && input.Length > 0)
                {
                    var s = input.Split(' ')[0];

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
