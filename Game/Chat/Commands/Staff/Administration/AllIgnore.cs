using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class AllIgnore : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                return;
            }

            if (TargetUser.GetUser().Rank >= Session.GetUser().Rank)
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

            TargetUser.GetUser().IgnoreAllExpireTime = expireTime;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                BanDao.InsertBan(dbClient, expireTime, "ignoreall", TargetUser.GetUser().Username, reason, Session.GetUser().Username);
        }
    }
}
