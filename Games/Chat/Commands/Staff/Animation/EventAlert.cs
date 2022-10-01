using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class EventAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
            {
                return;
            }

            string str = CommandManager.MergeParams(Params, 1);
            str = "<b>[ANIMATION]</b>\r\n" + str + "\r\n- " + Session.GetUser().Username;
            WibboEnvironment.GetGame().GetClientManager().SendSuperNotif("Animation des Staffs", str, "game_promo_small", "event:navigator/goto/" + UserRoom.RoomId, "Rejoindre!", true, true);
        }
    }
}
