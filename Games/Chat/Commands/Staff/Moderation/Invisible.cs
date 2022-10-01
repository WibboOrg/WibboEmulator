using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Invisible : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser().SpectatorMode)
            {
                Session.GetUser().SpectatorMode = false;
                Session.GetUser().HideInRoom = false;
            }
            else
            {
                Session.GetUser().SpectatorMode = true;
                Session.GetUser().HideInRoom = true;
            }

            Session.SendPacket(new GetGuestRoomResultComposer(Session, Room.RoomData, false, true));
        }
    }
}
