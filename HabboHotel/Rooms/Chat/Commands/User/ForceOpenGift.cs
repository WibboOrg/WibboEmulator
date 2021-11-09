using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class ForceOpenGift : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetHabbo().forceOpenGift = !Session.GetHabbo().forceOpenGift;

            if (Session.GetHabbo().forceOpenGift)
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
