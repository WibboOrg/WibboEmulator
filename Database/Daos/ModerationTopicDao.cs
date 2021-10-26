using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ModerationTopicDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, caption FROM moderation_topics");
            ModerationTopics = dbClient.GetTable();
        }
    }
}