namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Balayette : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = room.RoomUserManager.GetRoomUserByName(parameters[1]);
        if (targetUser == null || targetUser.Client == null || targetUser.Client.User == null)
        {
            return;
        }

        if (targetUser.Client.User.Id == session.User.Id)
        {
            return;
        }

        if (targetUser.Client.User.HasPremiumProtect && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", session.Language));
            return;
        }

        if (Math.Abs(targetUser.X - userRoom.X) >= 2 || Math.Abs(targetUser.Y - userRoom.Y) >= 2)
        {
            return;
        }

        var timeSpan = DateTime.Now - session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.fun.timeout", session.Language), 10 - (int)timeSpan.TotalSeconds));
            return;
        }

        session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.balayette.chat", session.Language), targetUser.Username), 32);
        targetUser.OnChat(string.Format(LanguageManager.TryGetValue("cmd.balayette.chat.target", session.Language), userRoom.Username), 18);

        if (targetUser.ContainStatus("lay") || targetUser.ContainStatus("sit"))
        {
            return;
        }

        if (targetUser.RotBody % 2 == 0 || targetUser.IsTransf)
        {
            if (targetUser.RotBody == 4 || targetUser.RotBody == 0 || targetUser.IsTransf)
            {
                if (room.GameMap.CanWalk(targetUser.X, targetUser.Y + 1))
                {
                    targetUser.RotBody = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (!room.GameMap.CanWalk(targetUser.X + 1, targetUser.Y))
                {
                    return;
                }
            }

            if (targetUser.IsTransf)
            {
                targetUser.SetStatus("lay", "0");
            }
            else
            {
                targetUser.SetStatus("lay", "0.7");
            }

            targetUser.IsLay = true;
            targetUser.UpdateNeeded = true;
        }
    }
}
