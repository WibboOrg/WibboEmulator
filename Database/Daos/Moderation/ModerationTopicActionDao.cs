using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
{
    class ModerationTopicActionDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `parent_id`, `type`, `caption`, `message_text`, `default_sanction`, `mute_time`, `ban_time`, `ip_time`, `trade_lock_time` FROM `moderation_topic_action`");
            return dbClient.GetTable();
        }
    }
}
