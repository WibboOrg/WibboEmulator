using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ForceOpenGift : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().ForceOpenGift = !Session.GetUser().ForceOpenGift;

            if (Session.GetUser().ForceOpenGift)
            {
                Session.SendWhisper("ForceOpenGift activé");
            }
            else
            {
                Session.SendWhisper("ForceOpenGift désactivé");
            }
        }
    }
}
