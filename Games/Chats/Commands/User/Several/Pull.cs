namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Pull : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (!room.PushPullAllowed)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var targetName = parameters[1];

        var TargetUser = room.RoomUserManager.GetRoomUserByName(targetName);
        if (TargetUser == null || TargetUser.Client == null || TargetUser.Client.User == null)
        {
            return;
        }

        if (TargetUser.Client.User.Id == Session.User.Id)
        {
            return;
        }

        if (TargetUser.Client.User.HasPremiumProtect && !Session.User.HasPermission("mod"))
        {
            Session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", Session.Language));
            return;
        }

        if (Math.Abs(userRoom.X - TargetUser.X) < 3 && Math.Abs(userRoom.Y - TargetUser.Y) < 3)
        {
            userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.pull.chat.success", Session.Language), targetName), 0, false);
            if (userRoom.RotBody % 2 != 0)
            {
                userRoom.RotBody--;
            }

            if (userRoom.RotBody == 0)
            {
                TargetUser.MoveTo(userRoom.X, userRoom.Y - 1);
            }
            else if (userRoom.RotBody == 2)
            {
                TargetUser.MoveTo(userRoom.X + 1, userRoom.Y);
            }
            else if (userRoom.RotBody == 4)
            {
                TargetUser.MoveTo(userRoom.X, userRoom.Y + 1);
            }
            else if (userRoom.RotBody == 6)
            {
                TargetUser.MoveTo(userRoom.X - 1, userRoom.Y);
            }
        }
        else
        {
            Session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.pull.fail", Session.Language), targetName));
            return;
        }
    }
}
