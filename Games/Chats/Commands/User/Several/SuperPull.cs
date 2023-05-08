namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SuperPull : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }
        
        var targetName = parameters[1];

        var targetUser = room.RoomUserManager.GetRoomUserByName(targetName);
        if (targetUser == null)
        {
            return;
        }

        if (targetUser.Client.User.Id == session.User.Id)
        {
            return;
        }

        if (targetUser.Client.User.PremiumProtect && !session.User.HasPermission("mod"))
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
            return;
        }

        if (userRoom.SetX - 1 == room.GameMap.Model.DoorX)
        {
            return;
        }

        userRoom.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.pull.chat.success", session.Langue), targetName), 0, false);
        if (userRoom.RotBody % 2 != 0)
        {
            userRoom.RotBody--;
        }

        if (userRoom.RotBody == 0)
        {
            targetUser.MoveTo(userRoom.X, userRoom.Y - 1);
        }
        else if (userRoom.RotBody == 2)
        {
            targetUser.MoveTo(userRoom.X + 1, userRoom.Y);
        }
        else if (userRoom.RotBody == 4)
        {
            targetUser.MoveTo(userRoom.X, userRoom.Y + 1);
        }
        else if (userRoom.RotBody == 6)
        {
            targetUser.MoveTo(userRoom.X - 1, userRoom.Y);
        }
    }
}
