namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceOpenGift : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.GetUser().ForceOpenGift = !session.GetUser().ForceOpenGift;

        if (session.GetUser().ForceOpenGift)
        {
            session.SendWhisper("ForceOpenGift activé");
        }
        else
        {
            session.SendWhisper("ForceOpenGift désactivé");
        }
    }
}
