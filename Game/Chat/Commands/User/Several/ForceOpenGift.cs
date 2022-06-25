using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class ForceOpenGift : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
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
