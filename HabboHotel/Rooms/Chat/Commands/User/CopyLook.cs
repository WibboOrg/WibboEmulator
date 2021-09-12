using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;

using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class CopyLook : IChatCommand    {        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {
            //if (UserRoom.team != Team.none || UserRoom.InGame)
            //return;

            if (Room.IsRoleplay && !Room.CheckRights(Session))
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }            string Username = Params[1];            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Username);
                if (Bot == null || Bot.BotData == null)
                {
                    return;
                }

                Session.GetHabbo().Gender = Bot.BotData.Gender;
                Session.GetHabbo().Look = Bot.BotData.Look;
            }
            else
            {

                if (clientByUsername.GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))
                {
                    UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                    return;
                }

                Session.GetHabbo().Gender = clientByUsername.GetHabbo().Gender;
                Session.GetHabbo().Look = clientByUsername.GetHabbo().Look;
            }            if (UserRoom.transformation || UserRoom.IsSpectator)
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;            if (currentRoom == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom;            if (roomUserByHabbo == null)
            {
                return;
            }

            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));            currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));        }    }}