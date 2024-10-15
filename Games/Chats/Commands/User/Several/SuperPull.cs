namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SuperPull : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var targetName = parameters[1];

        var TargetUser = room.RoomUserManager.GetRoomUserByName(targetName);
        if (TargetUser == null)
        {
            return;
        }

        if (TargetUser.Client.User.Id == Session.User.Id)
        {
            return;
        }

        if (TargetUser.Client.User.HasPremiumProtect && !Session.User.HasPermission("mod"))
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("premium.notallowed", Session.Language));
            return;
        }

        if (userRoom.SetX - 1 == room.GameMap.Model.DoorX)
        {
            return;
        }

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
}
