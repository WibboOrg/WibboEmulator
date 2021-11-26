using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class ForceOpenGift : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetHabbo().ForceOpenGift = !Session.GetHabbo().ForceOpenGift;

            if (Session.GetHabbo().ForceOpenGift)
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
