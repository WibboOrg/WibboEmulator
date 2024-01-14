namespace WibboEmulator.Database.Daos.Moderation;
using System.Data;
using Dapper;

internal sealed class ModerationPresetDao
{
    internal static List<ModerationPresetEntity> GetAll(IDbConnection dbClient) => dbClient.Query<ModerationPresetEntity>(
        "SELECT type, message FROM `moderation_preset` WHERE enabled = '1'"
    ).ToList();
}

public class ModerationPresetEntity
{
    public int Id { get; set; }
    public bool Enabled { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
}
