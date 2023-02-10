namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class EventAlert : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
        {
            return;
        }

        var str = CommandManager.MergeParams(parameters, 1);
        str = "<b>[ANIMATION]</b>\r\n" + str + "\r\n- " + session.User.Username;
        WibboEnvironment.GetGame().GetGameClientManager().SendSuperNotif("Animation des Staffs", str, "game_promo_small", "event:navigator/goto/" + userRoom.RoomId, "Rejoindre!");
    }
}
