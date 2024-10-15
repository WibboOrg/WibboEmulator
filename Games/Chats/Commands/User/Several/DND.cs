namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DND : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        Session.User.IgnoreRoomInvites = !Session.User.IgnoreRoomInvites;
        Session.SendWhisper("Tu " + (Session.User.IgnoreRoomInvites ? "acceptes" : "refuses") + " les messages dans ta console d'amis");
    }
}
