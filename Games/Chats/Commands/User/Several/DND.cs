namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DND : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.User.IgnoreRoomInvites = !session.User.IgnoreRoomInvites;
        session.SendWhisper("Tu " + (session.User.IgnoreRoomInvites ? "acceptes" : "refuses") + " les messages dans ta console d'amis");
    }
}
