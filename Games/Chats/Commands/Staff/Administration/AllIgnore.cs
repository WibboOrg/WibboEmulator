namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class AllIgnore : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (TargetUser == null || TargetUser.User == null)
        {
            return;
        }

        if (TargetUser.User.Rank >= Session.User.Rank)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("action.notallowed", Session.Language));
            return;
        }

        double lengthSeconds = -1;
        if (parameters.Length == 3)
        {
            _ = double.TryParse(parameters[2], out lengthSeconds);
        }

        if (lengthSeconds is <= 600 and > 0)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("ban.toolesstime", Session.Language));
            return;
        }

        var expireTime = lengthSeconds == -1 ? int.MaxValue : (int)(WibboEnvironment.GetUnixTimestamp() + lengthSeconds);
        var reason = CommandManager.MergeParams(parameters, 3);

        using var dbClient = DatabaseManager.Connection;
        var isIgnoreall = BanDao.GetOneIgnoreAll(dbClient, TargetUser.User.Id);
        if (isIgnoreall == 0)
        {
            BanDao.InsertBan(dbClient, expireTime, "ignoreall", TargetUser.User.Id.ToString(), reason, Session.User.Username);
        }

        TargetUser.User.IgnoreAllExpireTime = expireTime;

        Session.SendWhisper("Le joueur  " + TargetUser.User.Username + " est ignoré de la part de l'ensemble des joueurs pour " + reason + "!");
    }
}
