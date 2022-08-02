using WibboEmulator.Core;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Chat.Commands.Cmd;
using System.Data;
using System.Text;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Permissions;

namespace WibboEmulator.Game.Chat.Commands
{
    public class CommandManager
    {

        private readonly Dictionary<string, AuthorizationCommands> _commandRegisterInvokeable;
        private readonly Dictionary<string, string> _listCommande;
        private readonly Dictionary<int, IChatCommand> _commands;
        private readonly string _prefix = ":";

        public CommandManager()
        {
            _commands = new Dictionary<int, IChatCommand>();
            _commandRegisterInvokeable = new Dictionary<string, AuthorizationCommands>();
            _listCommande = new Dictionary<string, string>();
        }

        public void Init(IQueryAdapter dbClient)
        {

            InitInvokeableRegister(dbClient);

            RegisterCommand();
            RegisterPlayer();
            RegisterOwner();
            RegisterUserRight();
            RegisterPremium();
            RegisterAssStaffs();
            RegisterModeration();
            RegisterAnimation();
            RegisterAdministrator();
            RegisterGestion();
        }

        public bool Parse(Client Session, RoomUser User, Room Room, string Message)
        {
            if (Session == null || Session.GetUser() == null || Session.GetUser().CurrentRoom == null)
            {
                return false;
            }

            if (!Message.StartsWith(_prefix))
            {
                return false;
            }

            if (Message == _prefix + "commands")
            {
                Session.SendHugeNotif(WibboEnvironment.GetGame().GetChatManager().GetCommands().GetCommandList(Session));
                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
            {
                return false;
            }

            if (!this._commandRegisterInvokeable.TryGetValue(Split[0].ToLower(), out AuthorizationCommands CmdInfo))
            {
                return false;
            }

            if (!this._commands.TryGetValue(CmdInfo.CommandID, out IChatCommand Cmd))
            {
                return false;
            }

            int AutorisationType = CmdInfo.UserGotAuthorization2(Session, Room.RoomData.Langue);
            switch (AutorisationType)
            {
                case 2:
                    User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.premium", Session.Langue));
                    return true;
                case 3:
                    User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.accred", Session.Langue));
                    return true;
                case 4:
                    User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.owner", Session.Langue));
                    return true;
                case 5:
                    User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue", Session.Langue));
                    return true;
            }
            if (!CmdInfo.UserGotAuthorization(Session))
            {
                return false;
            }

            if (CmdInfo.UserGotAuthorizationStaffLog())
            {
                WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, Session.GetUser().CurrentRoomId, string.Empty, Split[0].ToLower(), string.Format("Tchat commande: {0}", string.Join(" ", Split)));
            }

            Cmd.Execute(Session, Session.GetUser().CurrentRoom, User, Split);
            return true;
        }

        private void InitInvokeableRegister(IQueryAdapter dbClient)
        {
            this._commandRegisterInvokeable.Clear();

            DataTable table = EmulatorCommandDao.GetAll(dbClient);

            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                int key = Convert.ToInt32(dataRow["id"]);
                int pRank = Convert.ToInt32(dataRow["minrank"]);
                string pDescriptionFr = (string)dataRow["description_fr"];
                string pDescriptionEn = (string)dataRow["description_en"];
                string pDescriptionBr = (string)dataRow["description_br"];
                string input = (string)dataRow["input"];
                string[] strArray = input.ToLower().Split(new char[1] { ',' });

                foreach (string command in strArray)
                {
                    if (this._commandRegisterInvokeable.ContainsKey(command))
                    {
                        continue;
                    }

                    this._commandRegisterInvokeable.Add(command, new AuthorizationCommands(key, strArray[0], pRank, pDescriptionFr, pDescriptionEn, pDescriptionBr));
                }
            }
        }

        public string GetCommandList(Client client)
        {
            string rank = client.GetUser().Rank + client.GetUser().Langue.ToString();
            if (this._listCommande.ContainsKey(rank))
            {
                return this._listCommande[rank];
            }

            List<string> NotDoublons = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (AuthorizationCommands chatCommand in this._commandRegisterInvokeable.Values)
            {
                if (chatCommand.UserGotAuthorization(client) && !NotDoublons.Contains(chatCommand.Input))
                {
                    if (client.Langue == Language.ANGLAIS)
                    {
                        stringBuilder.Append(":" + chatCommand.Input + " - " + chatCommand.DescriptionEn + "\r\r");
                    }
                    else if (client.Langue == Language.PORTUGAIS)
                    {
                        stringBuilder.Append(":" + chatCommand.Input + " - " + chatCommand.DescriptionBr + "\r\r");
                    }
                    else
                    {
                        stringBuilder.Append(":" + chatCommand.Input + " - " + chatCommand.DescriptionFr + "\r\r");
                    }

                    NotDoublons.Add(chatCommand.Input);
                }
            }
            NotDoublons.Clear();

            this._listCommande.Add(rank, (stringBuilder).ToString());
            return (stringBuilder).ToString();
        }

        public void Register(int CommandId, IChatCommand Command)
        {
            this._commands.Add(CommandId, Command);
        }


        public static string MergeParams(string[] Params, int Start)
        {
            StringBuilder Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(' ');

                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void RegisterOwner()
        {
            this.Register(1, new Pickall());
            this.Register(2, new Unload());
            this.Register(3, new KickAll());
            this.Register(4, new RoomFreeze());
            this.Register(5, new MaxFloor());
            this.Register(6, new AutoFloor());
        }

        public void RegisterUserRight()
        {
            this.Register(7, new SetSpeed());
            this.Register(8, new DisableDiagonal());
            this.Register(9, new SetMax());
            this.Register(10, new Override());
            this.Register(11, new Teleport());
            this.Register(12, new Freeze());
            this.Register(13, new RoomMute());
            this.Register(14, new Warp());
            this.Register(15, new RoomMutePet());
            this.Register(16, new SuperPull());
            this.Register(17, new ForceTransfStop());
            this.Register(18, new MakeSayBot());
            this.Register(19, new SetZ());
            this.Register(20, new SetZStop());
            this.Register(21, new DisableOblique());
            //this.Register(22, new BreakWalk());
            this.Register(23, new HideWireds());
            this.Register(24, new AllWarp());
            this.Register(25, new Use());
            this.Register(26, new Youtube());
            this.Register(27, new OldFoot());
            this.Register(28, new HidePyramide());
            this.Register(29, new ConfigBot());
            this.Register(30, new FastWalk());
            this.Register(31, new StartQuestion());
            this.Register(32, new StopQuestion());
            this.Register(33, new RoomYouTube());
            this.Register(34, new Kick());
        }

        public void RegisterPlayer()
        {
            this.Register(35, new Coords());
            this.Register(36, new HandItem());
            this.Register(37, new Enable());
            //this.Register(38, new Commands());
            this.Register(39, new About());
            this.Register(40, new ForceRot());
            this.Register(41, new EmptyItems());
            this.Register(42, new Follow());
            this.Register(43, new MoonWalk());
            this.Register(44, new Push());
            this.Register(45, new Pull());
            this.Register(46, new Mimic());
            this.Register(47, new Sit());
            this.Register(48, new Lay());
            this.Register(49, new Transf());
            this.Register(50, new TransfStop());
            this.Register(51, new DisableExchange());
            this.Register(52, new DisableFriendRequests());
            this.Register(53, new GiveItem());
            this.Register(54, new FaceLess());
            this.Register(55, new EmptyPets());
            this.Register(56, new Construit());
            this.Register(57, new ConstruitStop());
            this.Register(58, new Trigger());
            this.Register(59, new EmptyBots());
            this.Register(60, new DisableFollow());
            this.Register(61, new InfoSuperWired());
            this.Register(62, new RockPaperScissors());
            this.Register(63, new Mazo());
            this.Register(64, new LoadVideo());
            this.Register(65, new UseStop());
            this.Register(66, new GunFire());
            this.Register(67, new Cac());
            this.Register(68, new Pan());
            this.Register(69, new Prison());
            this.Register(70, new GameAlert());
            this.Register(71, new Emblem());
            this.Register(72, new GiveMoney());
            this.Register(73, new TransfBig());
            this.Register(74, new TransfLittle());
            this.Register(75, new ForceOpenGift());
            this.Register(76, new CloseDice());
            this.Register(77, new DND());
            this.Register(78, new Dance());
        }

        public void RegisterPremium()
        {
            this.Register(79, new UserInfo());
            this.Register(80, new Info());
            this.Register(81, new FaceWalk());
            this.Register(82, new VipProtect());
            this.Register(83, new Vip());
            this.Register(84, new TransfBot());
            this.Register(85, new RandomLook());
            this.Register(86, new GameTime());

        }

        public void RegisterAssStaffs()
        {
            this.Register(87, new StaffAlert());
            this.Register(88, new StaffsOnline());

        }

        public void RegisterModeration()
        {
            this.Register(89, new RoomAlert());
            this.Register(90, new Invisible());
            this.Register(91, new Ban());
            this.Register(92, new Disconnect());
            this.Register(93, new SuperBan());
            this.Register(94, new RoomKick());
            this.Register(95, new Mute());
            this.Register(96, new UnMute());
            this.Register(97, new Alert());
            this.Register(98, new StaffKick());
            this.Register(99, new DeleteMission());
            this.Register(100, new Summon());
            this.Register(101, new BanIP());
            this.Register(102, new TeleportStaff());
            this.Register(103, new WarpStaff());
            this.Register(104, new DisableWhispers());
            this.Register(105, new ForceFlagUser());
            this.Register(106, new KickBan());
        }

        public void RegisterAnimation()
        {
            this.Register(107, new GiveLot());
            this.Register(108, new NotifTop());
        }

        public void RegisterAdministrator()
        {
            this.Register(109, new DisabledWibboGame());
            this.Register(110, new RoomBadge());
            this.Register(111, new GiveBadge());
            this.Register(112, new ForceEnable());
            this.Register(113, new RemoveBadge());
            this.Register(114, new RoomEnable());
            this.Register(115, new UnloadRoom());
            this.Register(116, new ShowGuide());
            this.Register(117, new DuplicateRoom());
            this.Register(118, new SuperBot());
            this.Register(119, new PlaySoundRoom());
            this.Register(120, new StopSoundRoom());
            //this.Register(121, new OpenWeb());
            this.Register(122, new RoomEffect());
            this.Register(123, new ForceSit());
            this.Register(124, new Give());
        }

        public void RegisterGestion()
        {
            this.Register(125, new HotelAlert());
            this.Register(126, new MassBadge());
            //this.Register(127, new UnBan());
            this.Register(128, new AddFilter());
            this.Register(129, new EventAlert());
            this.Register(130, new ForceControlUser());
            this.Register(131, new MakeSay());
            this.Register(132, new ForceMimic());
            this.Register(133, new AddPhoto());
            this.Register(134, new Update());
            this.Register(135, new AllWhisper());
            this.Register(136, new AllIgnore());
            this.Register(137, new StartGameJD());
            this.Register(138, new AllAroundMe());
            this.Register(139, new AllEyesOnMe());
            this.Register(140, new RoomDance());
            this.Register(141, new ForceTransf());
            this.Register(142, new ForceEnableUser());
            this.Register(143, new TransfBot());
            this.Register(144, new AllFriends());
            this.Register(145, new RegenMap());
            this.Register(146, new ShutDown());
            this.Register(147, new MachineBan());
            this.Register(148, new ExtraBox());
            this.Register(149, new RoomSell());
            this.Register(150, new RoomBuy());
            this.Register(151, new RoomRemoveSell());
            this.Register(152, new LoadRoomItems());
            this.Register(153, new RegenLTD());
            this.Register(154, new SummonAll());
        }

        public void RegisterCommand()
        {
            this._commands.Clear();
        }
    }
}