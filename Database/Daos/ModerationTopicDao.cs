using System.Data;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ModerationTopicDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, caption FROM moderation_topics");
            return dbClient.GetTable();
        }
    }
}