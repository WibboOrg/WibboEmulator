namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class AllIgnore : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null)
        {
            return;
        }

        if (targetUser.User.Rank >= session.User.Rank)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            return;
        }

        double lengthSeconds = -1;
        if (parameters.Length == 3)
        {
            _ = double.TryParse(parameters[2], out lengthSeconds);
        }

        if (lengthSeconds is <= 600 and > 0)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", session.Langue));
            return;
        }

        var expireTime = lengthSeconds == -1 ? int.MaxValue : (int)(WibboEnvironment.GetUnixTimestamp() + lengthSeconds);
        var reason = CommandManager.MergeParams(parameters, 3);

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        var isIgnoreall = BanDao.GetOneIgnoreAll(dbClient, targetUser.User.Id);
        if (isIgnoreall == 0)
        {
            BanDao.InsertBan(dbClient, expireTime, "ignoreall", targetUser.User.Id.ToString(), reason, session.User.Username);
        }

        targetUser.User.IgnoreAllExpireTime = expireTime;

        session.SendWhisper("Le joueur  " + targetUser.User.Username + " est ignoré de la part de l'ensemble des joueurs pour " + reason + "!");
    }
}
