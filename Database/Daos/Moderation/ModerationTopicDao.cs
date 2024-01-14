namespace WibboEmulator.Database.Daos.Moderation;
using System.Data;
using Dapper;

internal sealed class ModerationTopicDao
{
    internal static List<ModerationTopicEntity> GetAll(IDbConnection dbClient) => dbClient.Query<ModerationTopicEntity>(
        "SELECT `id`, `caption` FROM `moderation_topic`"
    ).ToList();
}

public class ModerationTopicEntity
{
    public int Id { get; set; }
    public string Caption { get; set; }
}