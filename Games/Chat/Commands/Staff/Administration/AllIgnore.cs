using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class AllIgnore : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client targetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (targetUser == null || targetUser.GetUser() == null)
            {
                return;
            }

            if (targetUser.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                return;
            }

            double lengthSeconds = 788922000;
            if (Params.Length == 3)
            {
                double.TryParse(Params[2], out lengthSeconds);
            }

            if (lengthSeconds <= 600)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
                return;
            }

            double expireTime = WibboEnvironment.GetUnixTimestamp() + lengthSeconds;
            string reason = CommandManager.MergeParams(Params, 3);

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            double isIgnoreall = BanDao.GetOneIgnoreAll(dbClient, targetUser.GetUser().Username);
            if (isIgnoreall == 0)
            {
                BanDao.InsertBan(dbClient, expireTime, "ignoreall", targetUser.GetUser().Username, reason, Session.GetUser().Username);
            }

            targetUser.GetUser().IgnoreAllExpireTime = expireTime;

            Session.SendWhisper("Tu as ignoreall " + targetUser.GetUser().Username + " pour " + reason + "!");
        }
    }
}
