namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Balayette : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = room.RoomUserManager.GetRoomUserByName(parameters[1]);
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

        if (Math.Abs(TargetUser.X - userRoom.X) >= 2 || Math.Abs(TargetUser.Y - userRoom.Y) >= 2)
        {
            return;
        }

        var timeSpan = DateTime.Now - Session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.fun.timeout", Session.Language), 10 - (int)timeSpan.TotalSeconds));
            return;
        }

        Session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.balayette.chat", Session.Language), TargetUser.Username), 32);
        TargetUser.OnChat(string.Format(LanguageManager.TryGetValue("cmd.balayette.chat.target", Session.Language), userRoom.Username), 18);

        if (TargetUser.ContainStatus("lay") || TargetUser.ContainStatus("sit"))
        {
            return;
        }

        if (TargetUser.RotBody % 2 == 0 || TargetUser.IsTransf)
        {
            if (TargetUser.RotBody == 4 || TargetUser.RotBody == 0 || TargetUser.IsTransf)
            {
                if (room.GameMap.CanWalk(TargetUser.X, TargetUser.Y + 1))
                {
                    TargetUser.RotBody = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (!room.GameMap.CanWalk(TargetUser.X + 1, TargetUser.Y))
                {
                    return;
                }
            }

            if (TargetUser.IsTransf)
            {
                TargetUser.SetStatus("lay", "0");
            }
            else
            {
                TargetUser.SetStatus("lay", "0.7");
            }

            TargetUser.IsLay = true;
            TargetUser.UpdateNeeded = true;
        }
    }
}
