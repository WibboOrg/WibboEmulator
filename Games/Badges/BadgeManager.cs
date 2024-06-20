namespace WibboEmulator.Games.Badges;

using WibboEmulator.Core.Settings;

public static class BadgeManager
{
    private static readonly List<string> DisallowedBadgePrefixes =
    [
        "MRUN",
        "WORLDRUNSAVE",
        "ACH_"
    ];
    private static readonly List<string> NotAllowed = [];

    public static void Initialize()
    {
        NotAllowed.Clear();
        AddDisallowedFromConfig();
    }

    private static void AddDisallowedFromConfig(string configString = "badge.not.allowed")
    {
        var badgeNotAllowed = SettingsManager.GetData<string>(configString);
        NotAllowed.AddRange(badgeNotAllowed?.Split(',') ?? []);
    }

    public static bool HaveNotAllowed(string badgeId) => NotAllowed.Contains(badgeId) || IsDisallowedByType(badgeId);

    private static bool IsDisallowedByType(string badgeId) => DisallowedBadgePrefixes.Exists(badgeId.StartsWith);
}
