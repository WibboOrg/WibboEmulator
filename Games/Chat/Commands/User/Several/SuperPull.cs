namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SuperPull : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var TargetUser = Room.GetRoomUserManager().GetRoomUserByName(Params[1]);
        if (TargetUser == null)
        {
            return;
        }

        if (TargetUser.GetClient().GetUser().Id == session.GetUser().Id)
        {
            return;
        }

        if (TargetUser.GetClient().GetUser().PremiumProtect && !session.GetUser().HasPermission("perm_mod"))
        {
            UserRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
            return;
        }

        if (UserRoom.SetX - 1 == Room.GetGameMap().Model.DoorX)
        {
            return;
        }

        UserRoom.OnChat("*Tire " + Params[1] + "*", 0, false);
        if (UserRoom.RotBody % 2 != 0)
        {
            UserRoom.RotBody--;
        }

        if (UserRoom.RotBody == 0)
        {
            TargetUser.MoveTo(UserRoom.X, UserRoom.Y - 1);
        }
        else if (UserRoom.RotBody == 2)
        {
            TargetUser.MoveTo(UserRoom.X + 1, UserRoom.Y);
        }
        else if (UserRoom.RotBody == 4)
        {
            TargetUser.MoveTo(UserRoom.X, UserRoom.Y + 1);
        }
        else if (UserRoom.RotBody == 6)
        {
            TargetUser.MoveTo(UserRoom.X - 1, UserRoom.Y);
        }
    }
}
