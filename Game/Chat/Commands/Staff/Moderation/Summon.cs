using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Summon : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetHabbo() == null)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
                return;
            }
            else if (TargetUser.GetHabbo().CurrentRoom != null && TargetUser.GetHabbo().CurrentRoom.Id == Session.GetHabbo().CurrentRoom.Id)
            {
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            TargetUser.GetHabbo().IsTeleporting = true;
            TargetUser.GetHabbo().TeleportingRoomID = currentRoom.RoomData.Id;
            TargetUser.GetHabbo().TeleporterId = 0;

            TargetUser.SendPacket(new GetGuestRoomResultComposer(TargetUser, currentRoom.RoomData, false, true));
        }

    }
}