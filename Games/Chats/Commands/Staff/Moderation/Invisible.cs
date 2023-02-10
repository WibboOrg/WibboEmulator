namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Invisible : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.SpectatorMode)
        {
            session.User.SpectatorMode = false;
            session.User.HideInRoom = false;
        }
        else
        {
            session.User.SpectatorMode = true;
            session.User.HideInRoom = true;
        }

        session.SendPacket(new GetGuestRoomResultComposer(session, room.RoomData, false, true));
    }
}
