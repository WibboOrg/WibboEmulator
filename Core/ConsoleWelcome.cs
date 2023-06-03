namespace WibboEmulator.Core;

public class ConsoleWelcome
{
    public static void Write()
    {
        Console.Title = "Wibbo Emulator";

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(@" __        __  ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(@"_   ");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write(@"_       ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(@"_             ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(@"");

        Console.WriteLine(@"");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(@" \ \      / / ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(@"(_) ");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write(@"| |__   ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(@"| |__     ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(@"___  ");

        Console.WriteLine(@"");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(@"  \ \ /\ / /  ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(@"| | ");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write(@"| '_ \  ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(@"| '_ \   ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(@"/ _ \ ");

        Console.WriteLine("");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(@"   \ V  V /   ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(@"| | ");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write(@"| |_) | ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(@"| |_) | ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(@"| (_) |");

        Console.WriteLine(@"");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(@"    \_/\_/    ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("|_| ");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write("|_.__/  ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("|_.__/   ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(@"\___/ ");

        Console.WriteLine("");
        Console.WriteLine("");

        Console.ForegroundColor = ConsoleColor.Blue;

        Console.WriteLine("https://wibbo.org/");
        Console.WriteLine("Credits : Butterfly and Plus Emulator.");
        Console.WriteLine("-Wibbo Dev");
        Console.WriteLine("");

        Console.ForegroundColor = ConsoleColor.White;
    }
}