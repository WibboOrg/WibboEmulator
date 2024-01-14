namespace WibboEmulator.Database.Daos.Moderation;
using System.Data;
using Dapper;

internal sealed class ModerationTopicActionDao
{
    internal static List<ModerationTopicActionEntity> GetAll(IDbConnection dbClient) => dbClient.Query<ModerationTopicActionEntity>(
        "SELECT `id`, `parent_id`, `type`, `caption`, `message_text`, `default_sanction`, `mute_time`, `ban_time`, `ip_time`, `trade_lock_time` FROM `moderation_topic_action`"
    ).ToList();
}

public class ModerationTopicActionEntity
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Type { get; set; }
    public string Caption { get; set; }
    public string MessageText { get; set; }
    public string DefaultSanction { get; set; }
    public int MuteTime { get; set; }
    public int BanTime { get; set; }
    public int IpTime { get; set; }
    public int TradeLockTime { get; set; }
}