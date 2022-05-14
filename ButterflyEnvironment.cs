using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Core;
using Butterfly.Core.FigureData;
using Butterfly.Database;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;
using Butterfly.Game.Users.Authenticator;
using Butterfly.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.IO;
using Butterfly.Communication.WebSocket;

namespace Butterfly
{
    public static class ButterflyEnvironment
    {
        private static ConfigurationData _configuration;
        private static WebSocketManager _webSocketManager;
        private static GameCore _game;
        private static DatabaseManager _datebaseManager;
        private static RCONSocket _rcon;
        private static FigureDataManager _figureManager;
        private static LanguageManager _languageManager;

        private static HttpClient _httpClient = new HttpClient();
        private static Random _random = new Random();
        private static readonly ConcurrentDictionary<int, User> _usersCached = new ConcurrentDictionary<int, User>();

        public static DateTime ServerStarted;
        public static bool StaticEvents;
        public static string PatchDir;
        public static string CameraUploadUrl;
        public static string CameraThubmailUploadUrl;

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
            Console.Clear();

            ServerStarted = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.Gray;

            PatchDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/";

            Console.Title = "Butterfly Emulator";

            Console.ForegroundColor = ConsoleColor.DarkBlue;

            Console.WriteLine(@"  ____  _    _ _______ _______ ______ _____  ______ _  __     __ ");
            Console.WriteLine(@" |  _ \| |  | |__   __|__   __|  ____|  __ \|  ____| | \ \   / / ");
            Console.WriteLine(@" | |_) | |  | |  | |     | |  | |__  | |__) | |__  | |  \ \_/ /  ");
            Console.WriteLine(@" |  _ <| |  | |  | |     | |  |  __| |  _  /|  __| | |   \   /   ");
            Console.WriteLine(@" | |_) | |__| |  | |     | |  | |____| | \ \| |    | |____| |    ");
            Console.WriteLine(@" |____/ \____/   |_|     |_|  |______|_|  \_\_|    |______|_|    ");
            Console.WriteLine("");


            Console.WriteLine("     Butterfly Emulator Edition Wibbo");
            Console.WriteLine("     https://www.wibbo.org/");
            Console.WriteLine("     Credits : Butterfly and Plus Emulator.");
            Console.WriteLine("     - Jason Dhose");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.White;

            try
            {
                _configuration = new ConfigurationData(PatchDir + "configuration/settings.ini", false);
                _datebaseManager = new DatabaseManager(uint.Parse(GetConfig().data["db.pool.maxsize"]), uint.Parse(GetConfig().data["db.pool.minsize"]), GetConfig().data["db.hostname"], uint.Parse(GetConfig().data["db.port"]), GetConfig().data["db.username"], GetConfig().data["db.password"], GetConfig().data["db.name"]);


                int TryCount = 0;
                while (!_datebaseManager.IsConnected())
                {
                    TryCount++;
                    Thread.Sleep(5000);

                    if (TryCount > 10)
                    {
                        ExceptionLogger.WriteLine("Failed to connect to the specified MySQL server.");
                        Console.ReadKey(true);
                        Environment.Exit(1);
                        return;
                    }
                }

                _languageManager = new LanguageManager();
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    _languageManager.Init(dbClient);

                _game = new GameCore();
                _game.StartGameLoop();

                _figureManager = new FigureDataManager();
                _figureManager.Init();

                //_connectionManager = new ConnectionHandeling(int.Parse(GetConfig().data["game.tcp.port"]), int.Parse(GetConfig().data["game.tcp.conlimit"]), int.Parse(GetConfig().data["game.tcp.conperip"]));
                _webSocketManager = new WebSocketManager(int.Parse(GetConfig().data["game.ws.port"]), GetConfig().data["game.ssl.enable"] == "true", GetConfig().data["game.ssl.password"]);

                if (_configuration.data["mus.tcp.enable"] == "true")
                {
                    _rcon = new RCONSocket(int.Parse(GetConfig().data["mus.tcp.port"]), GetConfig().data["mus.tcp.allowedaddr"].Split(new char[1] { ';' }));
                }

                StaticEvents = _configuration.data["static.events"] == "true";
                CameraUploadUrl = _configuration.data["camera.upload.url"];
                CameraThubmailUploadUrl = _configuration.data["camera.thubmail.upload.url"];

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

        public static FigureDataManager GetFigureManager()
        {
            return _figureManager;
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
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                int integer = UserDao.GetIdByName(dbClient, username);
                if (integer <= 0)
                {
                    return false;
                }

                return true;
            }
        }

        public static string GetUsernameById(int UserId)
        {
            string Name = "Unknown User";

            Client Client = GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client != null && Client.GetUser() != null)
            {
                return Client.GetUser().Username;
            }

            if (_usersCached.ContainsKey(UserId))
            {
                return _usersCached[UserId].Username;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                Name = UserDao.GetNameById(dbClient, UserId);

            if (string.IsNullOrEmpty(Name))
            {
                Name = "Unknown User";
            }

            return Name;
        }

        public static User GetUserByUsername(string UserName)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                int id = UserDao.GetIdByName(dbClient, UserName);
                if (id > 0)
                {
                    return GetUserById(Convert.ToInt32(id));
                }

                return null;
            }
        }

        public static User GetUserById(int UserId)
        {
            try
            {
                Client Client = GetGame().GetClientManager().GetClientByUserID(UserId);
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

        public static LanguageManager GetLanguageManager()
        {
            return _languageManager;
        }

        public static ConfigurationData GetConfig()
        {
            return _configuration;
        }

        public static WebSocketManager GetWebSocketManager()
        {
            return _webSocketManager;
        }

        public static RCONSocket GetRCONSocket()
        {
            return _rcon;
        }

        public static GameCore GetGame()
        {
            return _game;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return _datebaseManager;
        }

        public static HttpClient GetHttpClient()
        {
            return _httpClient;
        }

        public static void PreformShutDown()
        {
            Console.Clear();
            Console.WriteLine("Extinction du serveur...");

            Console.Title = "BUTTERFLY : EXTINCTION";

            GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer("<b><font color=\"#ba3733\">Hôtel en cours de redémarrage</font></b><br><br>L'hôtel redémarrera dans 20 secondes. Nous nous excusons pour la gêne occasionnée.<br>Merci de ta visite, nous serons de retour dans environ 5 minutes."));
            GetGame().Destroy();
            Thread.Sleep(20000); // 20 secondes
            GetWebSocketManager().Destroy(); // Destruction
            GetGame().GetPacketManager().UnregisterAll(); // Dé-enregistrer les derniers achats du catalogue (uniquement mobis)
            GetGame().GetClientManager().CloseAll(); // Fermeture de toutes les connexions
            GetGame().GetRoomManager().RemoveAllRooms(); // Remise à zéro de la table room du SQL. 127.0.0.1...

            Console.WriteLine("Butterfly Emulateur s'est parfaitement éteint...");

            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}
