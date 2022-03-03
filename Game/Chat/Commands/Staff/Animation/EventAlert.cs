using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class EventAlert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!ButterflyEnvironment.GetGame().GetAnimationManager().AllowAnimation())
            {
                return;
            }

            string str = CommandManager.MergeParams(Params, 1);
            str = "<b>[ANIMATION]</b>\r\n" + str + "\r\n- " + Session.GetUser().Username;
            ButterflyEnvironment.GetGame().GetClientManager().SendSuperNotif("Animation des Staffs", str, "game_promo_small", "event:navigator/goto/" + UserRoom.RoomId, "Rejoindre!", true, true);
        }
    }
}
