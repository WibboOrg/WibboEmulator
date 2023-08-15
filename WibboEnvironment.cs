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
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Users.Authentificator;

public static class WibboEnvironment
{
    private static Game _game;
    private static WebSocketManager _webSocketManager;
    private static DatabaseManager _datebaseManager;
    private static RCONSocket _rcon;
    private static FigureDataManager _figureManager;
    private static LanguageManager _languageManager;
    private static SettingsManager _settingsManager;
    private static OpenAIProxy _chatOpenAI;

    private static readonly Random RandomNumber = new();

    private static readonly HttpClient HttpClient = new();
    private static readonly ConcurrentDictionary<int, User> UsersCached = new();
    private static readonly List<char> Allowedchars = new(new[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
            '-', '.', '=', '?', '!', ':'
        });

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

            _datebaseManager = new DatabaseManager(databaseConfiguration);

            if (!_datebaseManager.IsConnected())
            {
                ExceptionLogger.WriteLine("Failed to connect to the specified MySQL server.");
                _ = Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }

            using var dbClient = GetDatabaseManager().GetQueryReactor();

            _settingsManager = new SettingsManager();
            _settingsManager.Init(dbClient);

            _languageManager = new LanguageManager();
            _languageManager.Init(dbClient);

            _figureManager = new FigureDataManager();
            _figureManager.Init();

            _chatOpenAI = new OpenAIProxy(_settingsManager.GetData<string>("openia.api.key"));

            _game = new Game();
            _game.Init(dbClient);
            _game.StartGameLoop();

            var webSocketOrigins = _settingsManager.GetData<string>("game.ws.origins").Split(',').ToList();
            _webSocketManager = new WebSocketManager(_settingsManager.GetData<int>("game.ws.port"), _settingsManager.GetData<bool>("game.ssl.enable"), webSocketOrigins);

            if (_settingsManager.GetData<bool>("mus.tcp.enable"))
            {
                _rcon = new RCONSocket(_settingsManager.GetData<int>("mus.tcp.port"), _settingsManager.GetData<string>("mus.tcp.allowedaddr").Split(','));
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

    private static bool IsValid(char character) => Allowedchars.Contains(character);

    public static bool IsValidAlphaNumeric(string inputStr)
    {
        if (string.IsNullOrEmpty(inputStr))
        {
            return false;
        }

        for (var index = 0; index < inputStr.Length; ++index)
        {
            if (!IsValid(inputStr[index]))
            {
                return false;
            }
        }
        return true;
    }

    public static bool UsernameExists(string username)
    {
        using var dbClient = GetDatabaseManager().GetQueryReactor();
        var integer = UserDao.GetIdByName(dbClient, username);
        if (integer <= 0)
        {
            return false;
        }

        return true;
    }

    public static User GetUserById(int userId)
    {
        if (userId == 0)
        {
            return null;
        }

        try
        {
            var client = GetGame().GetGameClientManager().GetClientByUserID(userId);
            if (client != null)
            {
                var user = client.User;
                if (user != null && user.Id > 0)
                {
                    if (UsersCached.ContainsKey(userId))
                    {
                        _ = UsersCached.TryRemove(userId, out _);
                    }

                    return user;
                }
            }
            else
            {
                if (UsersCached.TryGetValue(userId, out var value))
                {
                    return value;
                }
                else
                {
                    var user = UserFactory.GetUserData(userId);
                    if (user != null)
                    {
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

    public static FigureDataManager GetFigureManager() => _figureManager;

    public static LanguageManager GetLanguageManager() => _languageManager;

    public static SettingsManager GetSettings() => _settingsManager;

    public static WebSocketManager GetWebSocketManager() => _webSocketManager;

    public static RCONSocket GetRCONSocket() => _rcon;

    public static Game GetGame() => _game;

    public static DatabaseManager GetDatabaseManager() => _datebaseManager;

    public static HttpClient GetHttpClient() => HttpClient;

    public static OpenAIProxy GetChatOpenAI() => _chatOpenAI;

    public static void PreformShutDown()
    {
        Console.Clear();
        Console.WriteLine("Extinction de l'émulateur.");

        GetGame().GetGameClientManager().SendMessage(new BroadcastMessageAlertComposer("<b><font color=\"#ba3733\">Hôtel en cours de redémarrage</font></b><br><br>L'hôtel redémarrera dans 20 secondes. Nous nous excusons pour la gêne occasionnée.<br>Merci de ta visite, nous serons de retour dans environ 3 minutes."));
        Thread.Sleep(20 * 1000); // 20 secondes
        //GetGame().StopGameLoop();
        GetWebSocketManager().Destroy(); // Eteindre le websocket server
        GetGame().GetPacketManager().UnregisterAll(); // Dé-enregistrer les packets
        GetGame().GetGameClientManager().CloseAll(); // Fermeture et enregistrement de toutes les utilisteurs
        GetGame().GetRoomManager().RemoveAllRooms(); // Fermerture et enregistrer des apparts
        GetGame().Dispose(); // E+N+D+T+I+M+E

        Console.WriteLine("Wibbo Emulateur s'est parfaitement éteint...");
        Environment.Exit(0);
    }
}
