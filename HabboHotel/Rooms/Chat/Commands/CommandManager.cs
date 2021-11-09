using Butterfly.Core;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        private readonly Dictionary<string, ChatCommand> _commandRegisterInvokeable;
        private readonly Dictionary<string, string> _listCommande;

        private readonly Dictionary<int, IChatCommand> _commands;

        public CommandManager()
        {
            this._commands = new Dictionary<int, IChatCommand>();
            this._commandRegisterInvokeable = new Dictionary<string, ChatCommand>();
            this._listCommande = new Dictionary<string, string>();
        }

        public void Init()
        {
            this.InitInvokeableRegister();
            this.RegisterCommand();
        }

        public bool Parse(GameClient Session, RoomUser User, Room Room, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
            {
                return false;
            }

            if (!Message.StartsWith(":"))
            {
                return false;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
            {
                return false;
            }

            if (!this._commandRegisterInvokeable.TryGetValue(Split[0].ToLower(), out ChatCommand CmdInfo))
            {
                return false;
            }

            if (!this._commands.TryGetValue(CmdInfo.commandID, out IChatCommand Cmd))
            {
                return false;
            }

            int AutorisationType = CmdInfo.UserGotAuthorization2(Session, Room.RoomData.Langue);
            switch (AutorisationType)
            {
                case 2:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.premium", Session.Langue));
                    return true;
                case 3:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.accred", Session.Langue));
                    return true;
                case 4:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.owner", Session.Langue));
                    return true;
                case 5:
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue", Session.Langue));
                    return true;
            }
            if (!CmdInfo.UserGotAuthorization(Session))
            {
                return false;
            }

            if (CmdInfo.UserGotAuthorizationStaffLog())
            {
                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().CurrentRoomId, string.Empty, Split[0].ToLower(), string.Format("Tchat commande: {0}", string.Join(" ", Split)));
            }

            Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, User, Split);
            return true;
        }

        private void InitInvokeableRegister()
        {
            this._commandRegisterInvokeable.Clear();

            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                table = EmulatorCommandDao.GetAll(dbClient);
                
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

                    this._commandRegisterInvokeable.Add(command, new ChatCommand(key, strArray[0], pRank, pDescriptionFr, pDescriptionEn, pDescriptionBr));
                }
            }
        }

        public string GetCommandList(GameClient client)
        {
            string rank = client.GetHabbo().Rank + client.GetHabbo().Langue.ToString();
            if (this._listCommande.ContainsKey(rank))
            {
                return this._listCommande[rank];
            }

            List<string> NotDoublons = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (ChatCommand chatCommand in this._commandRegisterInvokeable.Values)
            {
                if (chatCommand.UserGotAuthorization(client) && !NotDoublons.Contains(chatCommand.input))
                {
                    if (client.Langue == Language.ANGLAIS)
                    {
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionEn + "\r\r");
                    }
                    else if (client.Langue == Language.PORTUGAIS)
                    {
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionBr + "\r\r");
                    }
                    else
                    {
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionFr + "\r\r");
                    }

                    NotDoublons.Add(chatCommand.input);
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
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < Params.Length; ++index)
            {
                if (index >= Start)
                {
                    if (index > Start)
                    {
                        stringBuilder.Append(" ");
                    }

                    stringBuilder.Append(Params[index]);
                }
            }
            return (stringBuilder).ToString();
        }

        public void RegisterCommand()
        {
            this._commands.Clear();

            this.Register(1, new Pickall());
            this.Register(2, new SetSpeed());
            this.Register(3, new Unload());
            this.Register(4, new DisableDiagonal());
            this.Register(5, new SetMax());
            this.Register(6, new Override());
            this.Register(7, new Teleport());
            this.Register(8, new StaffAlert());
            this.Register(10, new RoomAlert());
            this.Register(11, new Coords());
            this.Register(12, new Coins());
            this.Register(14, new HandItem());
            this.Register(15, new HotelAlert());
            this.Register(16, new Freeze());
            this.Register(18, new Enable());
            this.Register(19, new RoomMute());
            this.Register(23, new RoomBadge());
            this.Register(24, new MassBadge());
            this.Register(26, new UserInfo());
            this.Register(28, new Shutdown());
            this.Register(30, new GiveBadge());
            this.Register(31, new Invisible());
            this.Register(32, new Ban());
            this.Register(33, new Disconnect());
            this.Register(34, new SuperBan());
            this.Register(36, new RoomKick());
            this.Register(37, new Mute());
            this.Register(38, new UnMute());
            this.Register(39, new Alert());
            this.Register(40, new Kick());
            this.Register(41, new Cmd.Commands());
            this.Register(42, new About());
            this.Register(43, new Info());
            this.Register(52, new ForceRot());
            this.Register(53, new SetEffect());
            this.Register(54, new EmptyItems());
            this.Register(60, new Warp());
            this.Register(61, new DeleteMission());
            this.Register(62, new Follow());
            this.Register(63, new Come());
            this.Register(64, new MoonWalk());
            this.Register(65, new Push());
            this.Register(66, new Pull());
            this.Register(67, new CopyLook());
            this.Register(69, new Sit());
            this.Register(70, new Lay());
            this.Register(84, new Transf());
            this.Register(85, new TransfStop());
            this.Register(86, new KickAll());
            this.Register(87, new Troc());
            this.Register(88, new Textamigo());
            this.Register(89, new BanIP());
            this.Register(90, new GiveItem());
            this.Register(91, new RoomMutePet());
            this.Register(92, new Facewalk());
            this.Register(94, new AddFilter());
            this.Register(95, new NoFace());
            this.Register(96, new EmptyPets());
            this.Register(97, new Construit());
            this.Register(98, new ConstruitStop());
            this.Register(100, new Spull());
            this.Register(101, new TeleportStaff());
            this.Register(102, new Trigger());
            this.Register(105, new RoomFreeze());
            this.Register(106, new RemoveBadge());
            this.Register(107, new RoomEnable());
            this.Register(108, new VipProtect());
            this.Register(109, new MachineBan());
            this.Register(111, new UnloadRoom());
            this.Register(112, new WarpStaff());
            this.Register(115, new EventAlert());
            this.Register(116, new Control());
            this.Register(117, new Say());
            this.Register(118, new SetCopyLook());
            this.Register(119, new SetTransf());
            this.Register(120, new SetTransfStop());
            this.Register(121, new SetEnable());
            this.Register(122, new Givelot());
            this.Register(123, new extrabox());
            this.Register(124, new SayBot());
            this.Register(126, new SetZ());
            this.Register(127, new SetZStop());
            this.Register(128, new Murmur());
            this.Register(130, new EmptyBots());
            this.Register(132, new Vip());
            this.Register(133, new FollowMe());
            this.Register(134, new DisableOblique());
            this.Register(135, new AddPhoto());
            this.Register(137, new AddPhoto());
            this.Register(138, new InfoSuperWired());
            this.Register(140, new TransfBot());
            this.Register(141, new SetTransfBot());
            this.Register(143, new ShowGuide());
            this.Register(144, new Cmd.Janken());
            this.Register(145, new RandomLook());
            this.Register(146, new Mazo());
            this.Register(148, new LoadVideo());
            this.Register(149, new HideWireds());
            this.Register(150, new WarpAll());
            this.Register(151, new Use());
            this.Register(152, new UseStop());
            this.Register(153, new Youtube());
            this.Register(154, new RoomSell());
            this.Register(155, new RoomBuy());
            this.Register(156, new RoomRemoveSell());
            this.Register(157, new DupliRoom());
            this.Register(158, new Tir());
            this.Register(159, new SuperBot());
            this.Register(160, new OldFoot());
            this.Register(161, new Pyramide());
            this.Register(162, new Cac());
            this.Register(163, new Pan());
            this.Register(165, new Prison());
            this.Register(166, new Refresh());
            this.Register(167, new GameAlert());
            this.Register(168, new MaxFloor());
            this.Register(169, new AutoFloor());
            this.Register(170, new Emblem());
            this.Register(171, new GiveMoney());
            this.Register(172, new ConfigBot());
            this.Register(173, new SpeedWalk());
            this.Register(174, new ChutAll());
            this.Register(175, new Flagme());
            this.Register(176, new IgnoreAll());
            this.Register(177, new PushNotif());
            this.Register(178, new Big());
            this.Register(179, new Little());
            this.Register(180, new LoadRoomItems());
            this.Register(181, new RegenLTD());
            this.Register(182, new ForceOpenGift());
            this.Register(183, new AllFriends());
            this.Register(184, new GameTime());
            this.Register(185, new StartQuestion());
            this.Register(186, new StopQuestion());
            this.Register(187, new PlaySoundRoom());
            this.Register(188, new StopSoundRoom());
            this.Register(189, new YoutubeRoom());
            this.Register(190, new CloseDice());
            this.Register(191, new OpenWeb());
            this.Register(192, new RoomEffect());
            this.Register(193, new KickBan());
            this.Register(194, new StartGameJD());
            this.Register(195, new RegenMaps());
            this.Register(196, new AllAroundMe());
            this.Register(197, new AllEyesOnMe());
            this.Register(198, new StaffsOnline());
        }
    }
}