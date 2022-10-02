using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core;
using WibboEmulator.Core.FigureData;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.GameClients.Authenticator;
using WibboEmulator.Net;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using WibboEmulator.Core.FigureData.JsonObject;
using Plus.Database;

namespace WibboEmulator;

public static class WibboEnvironment
{
    private static WebSocketManager _webSocketManager;
    private static Game _game;
    private static DatabaseManager _datebaseManager;
    private static RCONSocket _rcon;
    private static FigureDataManager _figureManager;
    private static LanguageManager _languageManager;
    private static SettingsManager _settingsManager;

    private static readonly HttpClient _httpClient = new HttpClient();
    private static Random _random = new Random();
    private static readonly ConcurrentDictionary<int, User> _usersCached = new ConcurrentDictionary<int, User>();

    public static DateTime ServerStarted { get; set; }
    public static List<string> WebSocketOrigins { get; set; }
    public static string PatchDir { get; set; }

    private static readonly List<char> Allowedchars = new List<char>(new[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
            '-', '.', '=', '?', '!', ':'
        });

    public static void Initialize()
    {
        ServerStarted = DateTime.Now;
        Console.ForegroundColor = ConsoleColor.Gray;

        PatchDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/";

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

        try
        {
            var jsonDatabase = File.ReadAllText(PatchDir + "Configuration/database.json");

            DatabaseConfiguration? databaseConfiguration = JsonSerializer.Deserialize<DatabaseConfiguration>(jsonDatabase);
            _datebaseManager = new DatabaseManager(databaseConfiguration);

            if (!_datebaseManager.IsConnected())
            {
                ExceptionLogger.WriteLine("Failed to connect to the specified MySQL server.");
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            _settingsManager = new SettingsManager();
            _settingsManager.Init(dbClient);

            _languageManager = new LanguageManager();
            _languageManager.Init(dbClient);

            _game = new Game();
            _game.StartGameLoop();

            _figureManager = new FigureDataManager();
            _figureManager.Init();

            WebSocketOrigins = _settingsManager.GetData<string>("game.ws.origins").Split(',').ToList();
            _webSocketManager = new WebSocketManager(_settingsManager.GetData<int>("game.ws.port"), _settingsManager.GetData<bool>("game.ssl.enable"));

            if (_settingsManager.GetData<bool>("mus.tcp.enable"))
            {
                _rcon = new RCONSocket(_settingsManager.GetData<int>("mus.tcp.port"), _settingsManager.GetData<string>("mus.tcp.allowedaddr").Split(','));
            }

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
            ExceptionLogger.WriteLine((ex).ToString());
            Console.ReadKey(true);
        }
        catch (InvalidOperationException ex)
        {
            ExceptionLogger.WriteLine("Failed to initialize ButterflyEmulator: " + ex.Message);
            ExceptionLogger.WriteLine("Press any key to shut down ...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fatal error during startup: " + (ex).ToString());
            Console.WriteLine("Press a key to exit");
            Console.ReadKey();
            Environment.Exit(1000);
        }
    }

    public static void RegenRandom()
    {
        _random = new Random();
    }

    public static bool EnumToBool(string Enum)
    {
        return Enum == "1";
    }

    public static string BoolToEnum(bool Bool)
    {
        return Bool ? "1" : "0";
    }

    public static int GetRandomNumber(int Min, int Max)
    {
        lock (_random) // synchronize
        {
            return _random.Next(Min, Max + 1);
        }
    }

    public static int GetUnixTimestamp()
    {
        return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
    }

    private static bool IsValid(char character)
    {
        return Allowedchars.Contains(character);
    }

    public static bool IsValidAlphaNumeric(string inputStr)
    {
        if (string.IsNullOrEmpty(inputStr))
        {
            return false;
        }

        for (int index = 0; index < inputStr.Length; ++index)
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
        using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        int integer = UserDao.GetIdByName(dbClient, username);
        if (integer <= 0)
        {
            return false;
        }

        return true;
    }

    public static string GetUsernameById(int UserId)
    {
        string Name = "Unknown User";

        GameClient Client = GetGame().GetGameClientManager().GetClientByUserID(UserId);
        if (Client != null && Client.GetUser() != null)
        {
            return Client.GetUser().Username;
        }

        if (_usersCached.ContainsKey(UserId))
        {
            return _usersCached[UserId].Username;
        }

        using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            Name = UserDao.GetNameById(dbClient, UserId);

        if (string.IsNullOrEmpty(Name))
        {
            Name = "Unknown User";
        }

        return Name;
    }

    public static User GetUserByUsername(string UserName)
    {
        using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        int id = UserDao.GetIdByName(dbClient, UserName);
        if (id > 0)
        {
            return GetUserById(Convert.ToInt32(id));
        }

        return null;
    }

    public static User GetUserById(int UserId)
    {
        try
        {
            GameClient Client = GetGame().GetGameClientManager().GetClientByUserID(UserId);
            if (Client != null)
            {
                User User = Client.GetUser();
                if (User != null && User.Id > 0)
                {
                    if (_usersCached.ContainsKey(UserId))
                    {
                        _usersCached.TryRemove(UserId, out User);
                    }

                    return User;
                }
            }
            else
            {
                try
                {
                    if (_usersCached.ContainsKey(UserId))
                    {
                        return _usersCached[UserId];
                    }
                    else
                    {
                        User user = UserFactory.GetUserData(UserId);
                        if (user != null)
                        {
                            _usersCached.TryAdd(UserId, user);
                            return user;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null; 
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public static FigureDataManager GetFigureManager() =>  _figureManager;

    public static LanguageManager GetLanguageManager() => _languageManager;
    
    public static SettingsManager GetSettings() => _settingsManager;

    public static WebSocketManager GetWebSocketManager() => _webSocketManager;

    public static RCONSocket GetRCONSocket() => _rcon;

    public static Game GetGame() => _game;

    public static DatabaseManager GetDatabaseManager() => _datebaseManager;

    public static HttpClient GetHttpClient() => _httpClient;

    public static void PreformShutDown()
    {
        Console.Clear();
        Console.WriteLine("Extinction du serveur...");

        GetGame().GetGameClientManager().SendMessage(new BroadcastMessageAlertComposer("<b><font color=\"#ba3733\">Hôtel en cours de redémarrage</font></b><br><br>L'hôtel redémarrera dans 20 secondes. Nous nous excusons pour la gêne occasionnée.<br>Merci de ta visite, nous serons de retour dans environ 5 minutes."));
        Thread.Sleep(20 * 1000); // 20 secondes
        GetGame().StopGameLoop();
        GetWebSocketManager().Destroy(); // Eteindre le websocket server
        GetGame().GetPacketManager().UnregisterAll(); // Dé-enregistrer les packets
        GetGame().GetGameClientManager().CloseAll(); // Fermeture et enregistrement de toutes les utilisteurs
        GetGame().GetRoomManager().RemoveAllRooms(); // Fermerture et enregistrer des apparts

        Console.WriteLine("Wibbo Emulateur s'est parfaitement éteint...");
        Environment.Exit(0);
    }
}
