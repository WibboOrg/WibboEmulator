using Butterfly.Communication.Packets.Outgoing.Navigator;

using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Follow : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));            }            else if ((clientByUsername.GetHabbo().HideInRoom) && !Session.GetHabbo().HasFuse("fuse_mod"))            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", Session.Langue));            }            else            {                Room currentRoom = clientByUsername.GetHabbo().CurrentRoom;                if (currentRoom != null)                {                    Session.SendPacket(new GetGuestRoomResultComposer(Session, currentRoom.RoomData, false, true));                }            }        }    }}