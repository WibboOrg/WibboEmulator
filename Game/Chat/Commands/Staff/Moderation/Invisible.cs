using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Invisible : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (Session.GetHabbo().SpectatorMode)
            {
                Session.GetHabbo().SpectatorMode = false;
                Session.GetHabbo().HideInRoom = false;
            }
            else
            {
                Session.GetHabbo().SpectatorMode = true;
                Session.GetHabbo().HideInRoom = true;
            }
            Session.SendPacket(new GetGuestRoomResultComposer(Session, Room.RoomData, false, true));
        }
    }
}
