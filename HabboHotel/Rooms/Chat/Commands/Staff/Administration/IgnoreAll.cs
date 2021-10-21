using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class IgnoreAll : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                return;
            }

            if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));                return;            }

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

            clientByUsername.GetHabbo().IgnoreAllExpireTime = expireTime;

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO bans (bantype,value,reason,expire,added_by,added_date) VALUES (@rawvar, @var, @reason, '" + expireTime + "', @mod, UNIX_TIMESTAMP())");
                queryreactor.AddParameter("rawvar", "ignoreall");
                queryreactor.AddParameter("var", clientByUsername.GetHabbo().Username);
                queryreactor.AddParameter("reason", reason);
                queryreactor.AddParameter("mod", Session.GetHabbo().Username);
                queryreactor.RunQuery();
            }
        }
    }
}
