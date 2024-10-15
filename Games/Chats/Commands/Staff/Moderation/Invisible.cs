namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Invisible : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (Session.User.IsSpectator)
        {
            Session.User.IsSpectator = false;
            Session.User.HideInRoom = false;
        }
        else
        {
            Session.User.IsSpectator = true;
            Session.User.HideInRoom = true;
        }

        Session.SendPacket(new GetGuestRoomResultComposer(Session, room.RoomData, false, true));
    }
}
