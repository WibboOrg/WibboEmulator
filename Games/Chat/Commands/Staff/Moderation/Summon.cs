using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Summon : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
                return;
            }
            else if (TargetUser.GetUser().CurrentRoom != null && TargetUser.GetUser().CurrentRoom.Id == Session.GetUser().CurrentRoom.Id)
            {
                return;
            }

            Room currentRoom = Session.GetUser().CurrentRoom;
            TargetUser.GetUser().IsTeleporting = true;
            TargetUser.GetUser().TeleportingRoomID = currentRoom.RoomData.Id;
            TargetUser.GetUser().TeleporterId = 0;

            TargetUser.SendPacket(new GetGuestRoomResultComposer(TargetUser, currentRoom.RoomData, false, true));
        }
    }
}