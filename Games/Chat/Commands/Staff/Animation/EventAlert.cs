namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class EventAlert : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
        {
            return;
        }

        var str = CommandManager.MergeParams(Params, 1);
        str = "<b>[ANIMATION]</b>\r\n" + str + "\r\n- " + session.GetUser().Username;
        WibboEnvironment.GetGame().GetGameClientManager().SendSuperNotif("Animation des Staffs", str, "game_promo_small", "event:navigator/goto/" + UserRoom.RoomId, "Rejoindre!", true, true);
    }
}
