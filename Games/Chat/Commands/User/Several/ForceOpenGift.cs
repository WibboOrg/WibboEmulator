namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceOpenGift : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
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
