namespace WibboEmulator.Games.Moderation;

public static class BanTypeUtility
{
    public static ModerationBanType GetModerationBanType(string type) => type switch
    {
        "ip" => ModerationBanType.IP,
        "machine" => ModerationBanType.Machine,
        _ => ModerationBanType.User,
    };

    public static string FromModerationBanType(ModerationBanType type) => type switch
    {
        ModerationBanType.IP => "ip",
        ModerationBanType.Machine => "machine",
        _ => "user",
    };
}
