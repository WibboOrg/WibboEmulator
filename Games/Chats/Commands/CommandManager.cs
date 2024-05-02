namespace WibboEmulator.Games.Chats.Commands;
using System.Data;
using System.Text;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.Chats.Commands.Staff.Animation;
using WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.Chats.Commands.User.Casino;
using WibboEmulator.Games.Chats.Commands.User.Info;
using WibboEmulator.Games.Chats.Commands.User.Inventory;
using WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.Chats.Commands.User.Premium.Fun;
using WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Games.Chats.Commands.User.RP;
using WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Permissions;
using WibboEmulator.Games.Rooms;

public static class CommandManager
{
    private static readonly Dictionary<string, AuthorizationCommands> CommandRegisterInvokeable = [];
    private static readonly Dictionary<string, string> ListCommande = [];
    private static readonly Dictionary<int, IChatCommand> Commands = [];
    private static readonly string Prefix = ":";

    public static void Initialize(IDbConnection dbClient)
    {
        InitInvokeableRegister(dbClient);
        RegisterCommand();
    }

    public static bool Parse(GameClient session, RoomUser user, Room room, string message)
    {
        if (!message.StartsWith(Prefix))
        {
            return false;
        }

        if (message == Prefix + "commands")
        {
            session.SendHugeNotification(GetCommandList(session, room));
            return true;
        }

        message = message[1..];
        var split = message.Split(' ');

        if (split.Length == 0)
        {
            return false;
        }

        if (!CommandRegisterInvokeable.TryGetValue(split[0].ToLower(), out var cmdInfo))
        {
            return false;
        }

        if (!Commands.TryGetValue(cmdInfo.CommandID, out var cmd))
        {
            return false;
        }

        var autorisationType = cmdInfo.UserGotAuthorizationType(session, room);
        switch (autorisationType)
        {
            case 2:
                user.SendWhisperChat(LanguageManager.TryGetValue("cmd.authorized.premium", session.Language));
                return true;
            case 3:
                user.SendWhisperChat(LanguageManager.TryGetValue("cmd.authorized.accred", session.Language));
                return true;
            case 4:
                user.SendWhisperChat(LanguageManager.TryGetValue("cmd.authorized.owner", session.Language));
                return true;
            case 5:
                user.SendWhisperChat(LanguageManager.TryGetValue("cmd.authorized.langue", session.Language));
                return true;
        }
        if (!cmdInfo.UserGotAuthorization(session, room))
        {
            return false;
        }

        if (cmdInfo.UserGotAuthorizationStaffLog())
        {
            ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, room.Id, string.Empty, split[0].ToLower(), string.Format("Tchat commande: {0}", string.Join(" ", split)));
        }

        cmd.Execute(session, room, user, split);
        return true;
    }

    private static void InitInvokeableRegister(IDbConnection dbClient)
    {
        CommandRegisterInvokeable.Clear();

        var emulatorCommandList = EmulatorCommandDao.GetAll(dbClient);

        if (emulatorCommandList.Count == 0)
        {
            return;
        }

        foreach (var emulatorCommand in emulatorCommandList)
        {
            var key = emulatorCommand.Id;
            var rank = emulatorCommand.MinRank;
            var descriptionFr = emulatorCommand.DescriptionFr;
            var descriptionEn = emulatorCommand.DescriptionEn;
            var descriptionBr = emulatorCommand.DescriptionBr;
            var input = emulatorCommand.Input;
            var strArray = input.ToLower().Split(',');

            foreach (var command in strArray)
            {
                if (CommandRegisterInvokeable.ContainsKey(command))
                {
                    continue;
                }

                CommandRegisterInvokeable.Add(command, new AuthorizationCommands(key, strArray[0], rank, descriptionFr, descriptionEn, descriptionBr));
            }
        }
    }

    public static string GetCommandList(GameClient client, Room room)
    {
        var rank = client.User.Rank + client.User.Langue.ToString();
        if (ListCommande.TryGetValue(rank, out var value))
        {
            return value;
        }

        var notDoublons = new List<string>();
        var stringBuilder = new StringBuilder();

        foreach (var chatCommand in CommandRegisterInvokeable.Values)
        {
            if (chatCommand.UserGotAuthorization(client, room) && !notDoublons.Contains(chatCommand.Input))
            {
                if (client.Language == Language.English)
                {
                    _ = stringBuilder.Append(":" + chatCommand.Input + " - " + chatCommand.DescriptionEn + "\r\r");
                }
                else if (client.Language == Language.Portuguese)
                {
                    _ = stringBuilder.Append(":" + chatCommand.Input + " - " + chatCommand.DescriptionBr + "\r\r");
                }
                else
                {
                    _ = stringBuilder.Append(":" + chatCommand.Input + " - " + chatCommand.DescriptionFr + "\r\r");
                }

                notDoublons.Add(chatCommand.Input);
            }
        }
        notDoublons.Clear();

        ListCommande.Add(rank, stringBuilder.ToString());
        return stringBuilder.ToString();
    }

    public static void Register(int commandId, IChatCommand command) => Commands.Add(commandId, command);


    public static string MergeParams(string[] parameters, int start)
    {
        var merged = new StringBuilder();
        for (var i = start; i < parameters.Length; i++)
        {
            if (i > start)
            {
                _ = merged.Append(' ');
            }

            _ = merged.Append(parameters[i]);
        }

        return merged.ToString();
    }

    public static void RegisterCommand()
    {
        Commands.Clear();

        Register(1, new Pickall());
        Register(2, new Unload());
        Register(3, new KickAll());
        Register(4, new RoomFreeze());
        Register(5, new MaxFloor());
        Register(6, new AutoFloor());
        Register(157, new WiredLimit());
        Register(7, new SetSpeed());
        Register(8, new DisableDiagonal());
        Register(9, new SetMax());
        Register(10, new Override());
        Register(11, new Teleport());
        Register(12, new Freeze());
        Register(13, new RoomMute());
        Register(14, new Warp());
        Register(15, new RoomMutePet());
        Register(16, new SuperPull());
        Register(17, new ForceTransfStop());
        Register(18, new MakeSayBot());
        Register(19, new SetZ());
        Register(20, new SetZStop());
        Register(21, new DisableOblique());
        Register(23, new HideWireds());
        Register(24, new AllWarp());
        Register(25, new Use());
        Register(26, new Youtube());
        Register(27, new OldFoot());
        Register(28, new HidePyramide());
        Register(29, new ConfigBot());
        Register(30, new FastWalk());
        Register(31, new StartQuestion());
        Register(32, new StopQuestion());
        Register(33, new RoomYouTube());
        Register(34, new Kick());
        Register(35, new Coords());
        Register(36, new HandItem());
        Register(37, new Enable());
        Register(39, new About());
        Register(40, new ForceRot());
        Register(41, new EmptyItems());
        Register(42, new Follow());
        Register(43, new MoonWalk());
        Register(44, new Push());
        Register(45, new Pull());
        Register(46, new Mimic());
        Register(47, new Sit());
        Register(48, new Lay());
        Register(49, new Transf());
        Register(50, new TransfStop());
        Register(51, new DisableExchange());
        Register(52, new DisableFriendRequests());
        Register(53, new GiveItem());
        Register(54, new FaceLess());
        Register(55, new EmptyPets());
        Register(56, new Construit());
        Register(57, new ConstruitStop());
        Register(59, new EmptyBots());
        Register(60, new DisableFollow());
        Register(61, new InfoSuperWired());
        Register(62, new RockPaperScissors());
        Register(63, new Mazo());
        Register(65, new UseStop());
        Register(66, new GunFire());
        Register(67, new Cac());
        Register(68, new Pan());
        Register(69, new Prison());
        Register(71, new Emblem());
        Register(72, new GiveMoney());
        Register(73, new TransfBig());
        Register(74, new TransfLittle());
        Register(75, new ForceOpenGift());
        Register(76, new CloseDice());
        Register(77, new DND());
        Register(78, new Dance());
        Register(158, new ConvertMagot());
        Register(79, new UserInfo());
        Register(80, new Info());
        Register(81, new FaceWalk());
        Register(82, new VipProtect());
        Register(83, new Premium());
        Register(84, new TransfBot());
        Register(85, new RandomLook());
        Register(86, new GameTime());
        Register(159, new Balayette());
        Register(160, new Hug());
        Register(161, new Laser());
        Register(162, new Nuke());
        Register(163, new Slime());
        Register(164, new Tomato());
        Register(165, new Tied());
        Register(87, new StaffAlert());
        Register(88, new StaffsOnline());
        Register(89, new RoomAlert());
        Register(90, new Invisible());
        Register(91, new Ban());
        Register(92, new Disconnect());
        Register(93, new SuperBan());
        Register(94, new RoomKick());
        Register(95, new Mute());
        Register(96, new UnMute());
        Register(97, new Alert());
        Register(98, new StaffKick());
        Register(99, new DeleteMission());
        Register(100, new Summon());
        Register(101, new BanIP());
        Register(102, new TeleportStaff());
        Register(103, new WarpStaff());
        Register(104, new DisableWhispers());
        Register(105, new ForceFlagUser());
        Register(106, new KickBan());
        Register(107, new GiveLot());
        Register(108, new NotifTop());
        Register(169, new ShowInventory());
        Register(109, new DisabledAutoGame());
        Register(110, new RoomBadge());
        Register(111, new GiveBadge());
        Register(112, new ForceEnable());
        Register(113, new RemoveBadge());
        Register(114, new RoomEnable());
        Register(115, new UnloadRoom());
        Register(116, new ShowGuide());
        Register(117, new DuplicateRoom());
        Register(118, new SuperBot());
        Register(119, new PlaySoundRoom());
        Register(120, new StopSoundRoom());
        Register(122, new RoomEffect());
        Register(123, new ForceSit());
        Register(124, new Give());
        Register(125, new HotelAlert());
        Register(126, new MassBadge());
        Register(128, new AddFilter());
        Register(129, new EventAlert());
        Register(130, new ForceControlUser());
        Register(131, new MakeSay());
        Register(132, new ForceMimic());
        Register(133, new AddPhoto());
        Register(134, new Update());
        Register(135, new AllWhisper());
        Register(136, new AllIgnore());
        Register(137, new StartGameJD());
        Register(138, new AllAroundMe());
        Register(139, new AllEyesOnMe());
        Register(140, new RoomDance());
        Register(141, new ForceTransf());
        Register(142, new ForceEnableUser());
        Register(143, new TransfBot());
        Register(144, new AllFriends());
        Register(145, new RegenMap());
        Register(146, new ShutDown());
        Register(148, new ExtraBox());
        Register(149, new RoomSell());
        Register(150, new RoomBuy());
        Register(151, new RoomRemoveSell());
        Register(152, new LoadRoomItems());
        Register(153, new RegenLTD());
        Register(154, new SummonAll());
        Register(155, new LootboxInfo());
        Register(156, new UnloadEmptyRooms());
        Register(166, new GiveBanner());
        Register(167, new RemoveBanner());
        Register(168, new RoomBanner());
        Register(170, new ChatGPT());
        Register(171, new TransfertRoom());
        Register(172, new DeleteGroup());
        Register(173, new Turn());
        Register(175, new SuperPush());
        Register(176, new ChatAudio());
        Register(177, new ChatToSpeech());
        Register(178, new ChatToSpeechElevenlabs());
    }
}
