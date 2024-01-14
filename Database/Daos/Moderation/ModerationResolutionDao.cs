namespace WibboEmulator.Database.Daos.Moderation;
using System.Data;
using Dapper;

internal sealed class ModerationResolutionDao
{
    internal static List<ModerationResolutionEntity> GetAll(IDbConnection dbClient) => dbClient.Query<ModerationResolutionEntity>(
        "SELECT `id`, `type`, `title`, `subtitle`, `ban_hours`, `enable_mute`, `mute_hours`, `reminder`, `message` FROM `moderation_resolution`"
    ).ToList();
}

public class ModerationResolutionEntity
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public int BanHours { get; set; }
    public int EnableMute { get; set; }
    public int MuteHours { get; set; }
    public int Reminder { get; set; }
    public string Message { get; set; }
}