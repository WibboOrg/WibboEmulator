using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class EventAlert : IChatCommand
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
            if (!ButterflyEnvironment.GetGame().GetAnimationManager().AllowAnimation())
            {
                return;
            }

            string str = CommandManager.MergeParams(Params, 1);
            str = "<b>[ANIMATION]</b>\r\n" + str + "\r\n- " + Session.GetHabbo().Username;
            ButterflyEnvironment.GetGame().GetClientManager().SendSuperNotif("Animation des Staffs", str, "game_promo_small", "event:navigator/goto/" + UserRoom.RoomId, "Rejoindre!", true, true);
        }
    }
}
