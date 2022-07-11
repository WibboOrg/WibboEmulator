using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class LogStaffDao
    {
        internal static void Insert(IQueryAdapter dbClient, string name, string action)
        {
            dbClient.SetQuery("INSERT INTO `log_staff` (`pseudo`, `action`, `date`) VALUES (@name, @action, UNIX_TIMESTAMP())");
            dbClient.AddParameter("name", name);
            dbClient.AddParameter("action", action);
            dbClient.RunQuery();
        }
    }
}