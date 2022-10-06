namespace WibboEmulator.Games.Moderation;

public static class BanTypeUtility
{
    public static ModerationBanType GetModerationBanType(string type) => type switch
    {
        "ip" => ModerationBanType.IP,
        "machine" => ModerationBanType.MACHINE,
        _ => ModerationBanType.USER,
    };

    public static string FromModerationBanType(ModerationBanType type) => type switch
    {
        ModerationBanType.IP => "ip",
        ModerationBanType.MACHINE => "machine",
        _ => "user",
    };
}
