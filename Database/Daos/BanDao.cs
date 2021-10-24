using Butterfly.Database.Interfaces;
using System;

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
    }
}
