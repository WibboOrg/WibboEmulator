namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllIgnore : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
        if (targetUser == null || targetUser.GetUser() == null)
        {
            return;
        }

        if (targetUser.GetUser().Rank >= session.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
            return;
        }

        double lengthSeconds = 788922000;
        if (Params.Length == 3)
        {
            _ = double.TryParse(Params[2], out lengthSeconds);
        }

        if (lengthSeconds <= 600)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", session.Langue));
            return;
        }

        var expireTime = WibboEnvironment.GetUnixTimestamp() + lengthSeconds;
        var reason = CommandManager.MergeParams(Params, 3);

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var isIgnoreall = BanDao.GetOneIgnoreAll(dbClient, targetUser.GetUser().Username);
        if (isIgnoreall == 0)
        {
            BanDao.InsertBan(dbClient, expireTime, "ignoreall", targetUser.GetUser().Username, reason, session.GetUser().Username);
        }

        targetUser.GetUser().IgnoreAllExpireTime = expireTime;

        session.SendWhisper("Tu as ignoreall " + targetUser.GetUser().Username + " pour " + reason + "!");
    }
}
