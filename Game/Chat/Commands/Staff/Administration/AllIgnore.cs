using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class AllIgnore : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetHabbo() == null)
            {
                return;
            }

            if (TargetUser.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                return;
            }

            double lengthSeconds = 788922000;
            if (Params.Length == 3)
            {
                double.TryParse(Params[2], out lengthSeconds);
            }

            if (lengthSeconds <= 600)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
                return;
            }

            double expireTime = ButterflyEnvironment.GetUnixTimestamp() + lengthSeconds;

            string reason = CommandManager.MergeParams(Params, 3);

            TargetUser.GetHabbo().IgnoreAllExpireTime = expireTime;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                BanDao.InsertBan(dbClient, expireTime, "ignoreall", TargetUser.GetHabbo().Username, reason, Session.GetHabbo().Username);
        }
    }
}
