using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class BanDao
    {
        internal static bool IsBanned(IQueryAdapter dbClient, int nowTime, string ip)
        {
            dbClient.SetQuery("SELECT id FROM bans WHERE expire > @nowtime AND (bantype = 'ip' AND value = @ip) LIMIT 1");
            dbClient.AddParameter("nowtime", nowTime);
            dbClient.AddParameter("ip", ip);

            return dbClient.FindsResult();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id FROM bans WHERE expire > @nowtime AND ((bantype = 'user' AND value = @username) OR (bantype = 'ip' AND value = @IP1) OR (bantype = 'ip' AND value = @IP2) OR (bantype = 'machine' AND value = @machineid)) LIMIT 1");
            dbClient.AddParameter("nowtime", ButterflyEnvironment.GetUnixTimestamp());
            dbClient.AddParameter("username", dUserInfo["username"]);
            dbClient.AddParameter("IP1", ip);
            dbClient.AddParameter("IP2", dUserInfo["ip_last"]);
            dbClient.AddParameter("machineid", machineid);
            DataRow IsBanned = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT expire FROM bans WHERE bantype = 'ignoreall' AND value = @username");
            dbClient.AddParameter("username", dUserInfo["username"]);
            DataRow IgnoreAll = dbClient.GetRow();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO bans (bantype,value,reason,expire,added_by,added_date) VALUES (@rawvar, @var, @reason, '" + expireTime + "', @mod, UNIX_TIMESTAMP())");
            dbClient.AddParameter("rawvar", "ignoreall");
            dbClient.AddParameter("var", clientByUsername.GetHabbo().Username);
            dbClient.AddParameter("reason", reason);
            dbClient.AddParameter("mod", Session.GetHabbo().Username);
            dbClient.RunQuery();
        }
    }
}
