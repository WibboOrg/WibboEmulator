using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class ForceOpenGift : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
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
