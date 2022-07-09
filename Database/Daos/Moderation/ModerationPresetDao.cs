using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class ModerationPresetDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT type, message FROM `moderation_preset` WHERE enabled = '1'");
            return dbClient.GetTable();
        }
    }
}