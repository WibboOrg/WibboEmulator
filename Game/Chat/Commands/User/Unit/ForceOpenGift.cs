using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class ForceOpenGift : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().ForceOpenGift = !Session.GetUser().ForceOpenGift;

            if (Session.GetUser().ForceOpenGift)
            {
                UserRoom.SendWhisperChat("ForceOpenGift activé");
            }
            else
            {
                UserRoom.SendWhisperChat("ForceOpenGift désactivé");
            }
        }
    }
}
