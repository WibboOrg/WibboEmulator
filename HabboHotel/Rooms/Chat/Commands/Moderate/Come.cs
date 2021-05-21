using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Come : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
                return;
            }
            else if (clientByUsername.GetHabbo().CurrentRoom != null && clientByUsername.GetHabbo().CurrentRoom.Id == Session.GetHabbo().CurrentRoom.Id)
            {
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            clientByUsername.GetHabbo().IsTeleporting = true;
            clientByUsername.GetHabbo().TeleportingRoomID = currentRoom.RoomData.Id;
            clientByUsername.GetHabbo().TeleporterId = 0;

            clientByUsername.SendPacket(new GetGuestRoomResultComposer(clientByUsername, currentRoom.RoomData, false, true));
        }

    }
}