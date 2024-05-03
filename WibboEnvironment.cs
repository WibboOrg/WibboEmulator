namespace WibboEmulator;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.RCON;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core;
using WibboEmulator.Core.FigureData;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.OpenIA;
using WibboEmulator.Core.ElevenLabs;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Users.Authentificator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Communication.Packets;

public static class WibboEnvironment
{
    private static readonly Random RandomNumber = new();
    private static readonly ConcurrentDictionary<int, User> UsersCached = new();
    private static readonly List<char> Allowedchars = new(new[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
            '-', '.', '=', '!', ':', '@'
        });

    public static HttpClient HttpClient { get; } = new();
    public static DateTime ServerStarted { get; private set; }
    public static string PatchDir { get; private set; }

    public static void Initialize()
    {
        ServerStarted = DateTime.Now;
        Console.ForegroundColor = ConsoleColor.Gray;

        PatchDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) + "/";

        ConsoleWelcome.Write();

        try
        {
            var jsonDatabase = File.ReadAllText(PatchDir + "Configuration/database.json");

            var databaseConfiguration = JsonSerializer.Deserialize<DatabaseConfiguration>(jsonDatabase);

            if (databaseConfiguration == null)
            {
                ExceptionLogger.WriteLine("Failed to load Json MySQL.");
                _ = Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }

            DatabaseManager.Initialize(databaseConfiguration);

            if (!DatabaseManager.IsConnected)
            {
                ExceptionLogger.WriteLine("Failed to connect to the specified MySQL server.");
                _ = Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }

            using var dbClient = DatabaseManager.Connection;

            SettingsManager.Initialize(dbClient);
            LanguageManager.Initialize(dbClient);
            FigureDataManager.Initialize();
            OpenAIProxy.Initialize(SettingsManager.GetData<string>("openia.api.key"));
            ElevenLabsProxy.Initialize(SettingsManager.GetData<string>("elevenlabs.api.key"));

            Game.Initialize(dbClient);
            Game.StartGameLoop();

            var webSocketOrigins = SettingsManager.GetData<string>("game.ws.origins").Split(',').ToList();
            WebSocketManager.Initialize(SettingsManager.GetData<int>("game.ws.port"), SettingsManager.GetData<bool>("game.ssl.enable"), webSocketOrigins);

            if (SettingsManager.GetData<bool>("mus.tcp.enable"))
            {
                RCONSocket.Initialize(SettingsManager.GetData<int>("mus.tcp.port"), [.. SettingsManager.GetData<string>("mus.tcp.allowedaddr").Split(',')]);
            }

            HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Wibbo", "1.0"));
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(+http://wibbo.org)"));

            ExceptionLogger.WriteLine("EMULATOR -> READY!");

            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ExceptionLogger.WriteLine("Server is debugging: Console writing enabled");
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                ExceptionLogger.WriteLine("Server is not debugging: Console writing disabled");
                ExceptionLogger.DisablePrimaryWriting(false);
            }
        }
        catch (KeyNotFoundException ex)
        {
            ExceptionLogger.WriteLine("Please check your configuration file - some values appear to be missing.");
            ExceptionLogger.WriteLine("Press any key to shut down ...");
            ExceptionLogger.WriteLine(ex.ToString());
            _ = Console.ReadKey(true);
        }
        catch (InvalidOperationException ex)
        {
            ExceptionLogger.WriteLine("Failed to initialize ButterflyEmulator: " + ex.Message);
            ExceptionLogger.WriteLine("Press any key to shut down ...");
            _ = Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fatal error during startup: " + ex.ToString());
            Console.WriteLine("Press a key to exit");
            _ = Console.ReadKey();
            Environment.Exit(1000);
        }
    }

    public static int GetRandomNumber(int min, int max)
    {
        lock (RandomNumber) // synchronize
        {
            return RandomNumber.Next(min, max + 1);
        }
    }

    public static int GetUnixTimestamp() => (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static bool IsValidAlphaNumeric(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        return input.All(Allowedchars.Contains);
    }

    public static int NameAvailable(string username)
    {
        if (username.Length is < 3 or > 15 || !IsValidAlphaNumeric(username))
        {
            return -1;
        }

        return UsernameExists(username) ? 0 : 1;
    }

    public static bool UsernameExists(string username)
    {
        using var dbClient = DatabaseManager.Connection;
        var integer = UserDao.GetIdByName(dbClient, username);
        if (integer <= 0)
        {
            return false;
        }

        return true;
    }

    public static string GetNameById(int id)
    {
        var clientByUserId = GetUserById(id);

        if (clientByUserId != null)
        {
            return clientByUserId.Username;
        }

        using var dbClient = DatabaseManager.Connection;

        return UserDao.GetNameById(dbClient, id);
    }

    public static User GetUserById(int userId)
    {
        if (userId <= 0)
        {
            return null;
        }

        try
        {
            var client = GameClientManager.GetClientByUserID(userId);
            if (client != null)
            {
                var user = client.User;
                if (user != null && user.Id > 0)
                {
                    _ = UsersCached.TryRemove(userId, out _);
                    return user;
                }
            }
            else
            {
                if (UsersCached.TryGetValue(userId, out var cachedUser))
                {
                    return cachedUser;
                }
                else
                {
                    using var dbClient = DatabaseManager.Connection;
                    var user = UserFactory.GetUserData(dbClient, userId);

                    if (user != null)
                    {
                        user.InitializeProfile(dbClient);
                        _ = UsersCached.TryAdd(userId, user);
                        return user;
                    }
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public static void PerformShutDown()
    {
        Console.Clear();
        Console.WriteLine("Extinction de l'émulateur.");

        GameClientManager.SendMessage(new BroadcastMessageAlertComposer("<b><font color=\"#ba3733\">Hôtel en cours de redémarrage</font></b><br><br>L'hôtel redémarrera dans 20 secondes. Nous nous excusons pour la gêne occasionnée.<br>Merci de ta visite, nous serons de retour dans environ 3 minutes."));
        Thread.Sleep(20 * 1000);
        WebSocketManager.Destroy();
        PacketManager.UnregisterAll();
        GameClientManager.CloseAll();
        RoomManager.RemoveAllRooms();

        Console.WriteLine("L'émulateur s'est parfaitement éteinte.");
        Environment.Exit(0);
    }
}
