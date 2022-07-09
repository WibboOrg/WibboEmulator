using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class BanDao
    {
        internal static bool IsBanned(IQueryAdapter dbClient, string username, string ip, string ipTwo, string machineId)
        {
            dbClient.SetQuery("SELECT `id` FROM `ban` WHERE `expire` > @nowtime AND ((`bantype` = 'user' AND `value` = @username) OR (`bantype` = 'ip' AND `value` = @IP1) OR (`bantype` = 'ip' AND `value` = @IP2) OR (`bantype` = 'machine' AND `value` = @machineid)) LIMIT 1");
            dbClient.AddParameter("nowtime", WibboEnvironment.GetUnixTimestamp());
            dbClient.AddParameter("username", username);
            dbClient.AddParameter("IP1", ip);
            dbClient.AddParameter("IP2", ipTwo);
            dbClient.AddParameter("machineid", machineId);

            return dbClient.FindsResult();
        }

        internal static double GetOneIgnoreAll(IQueryAdapter dbClient, string username)
        {
            dbClient.SetQuery("SELECT `expire` FROM `ban` WHERE `bantype` = 'ignoreall' AND `value` = @username");
            dbClient.AddParameter("username", username);

            DataRow row = dbClient.GetRow();

            if (row == null) return 0;

            double.TryParse(row[0].ToString(), out double expire);

            return expire;
        }

        internal static void InsertBan(IQueryAdapter dbClient, double expireTime, string banType, string username, string reason, string modName)
        {
            dbClient.SetQuery("INSERT INTO `ban` (`bantype`,`value`,`reason`,`expire`,`added_by`,`added_date`) VALUES (@rawvar, @var, @reason, '" + expireTime + "', @mod, UNIX_TIMESTAMP())");
            dbClient.AddParameter("rawvar", banType);
            dbClient.AddParameter("var", username);
            dbClient.AddParameter("reason", reason);
            dbClient.AddParameter("mod", modName);
            dbClient.RunQuery();
        }
    }
}
