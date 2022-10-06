namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Invisible : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (session.GetUser().SpectatorMode)
        {
            session.GetUser().SpectatorMode = false;
            session.GetUser().HideInRoom = false;
        }
        else
        {
            session.GetUser().SpectatorMode = true;
            session.GetUser().HideInRoom = true;
        }

        session.SendPacket(new GetGuestRoomResultComposer(session, Room.RoomData, false, true));
    }
}
