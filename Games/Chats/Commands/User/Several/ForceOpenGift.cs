namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceOpenGift : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        Session.User.ForceOpenGift = !Session.User.ForceOpenGift;

        if (Session.User.ForceOpenGift)
        {
            Session.SendWhisper("ForceOpenGift activé");
        }
        else
        {
            Session.SendWhisper("ForceOpenGift désactivé");
        }
    }
}
