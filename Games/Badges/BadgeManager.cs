namespace WibboEmulator.Games.Badges;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class BadgeManager
{
    private static readonly Dictionary<string, Badge> BadgeList = [];

    public static void Initialize(IDbConnection dbClient)
    {
        BadgeList.Clear();

        var emulatorBadgeList = EmulatorBadgeDao.GetAll(dbClient);

        foreach (var emulatorBadge in emulatorBadgeList)
        {
            BadgeList.Add(emulatorBadge.Code, new Badge(emulatorBadge.Code, emulatorBadge.CanTrade, emulatorBadge.CanDelete, emulatorBadge.CanGive, emulatorBadge.AmountWinwins));
        }
    }

    public static bool TryGetBadgeByCode(string code, out Badge badge) => BadgeList.TryGetValue(code, out badge);

    public static bool CanGiveBadge(string badgeId) => TryGetBadgeByCode(badgeId, out var badge) && badge.CanGive;

    public static bool CanTradeBadge(string badgeId) => TryGetBadgeByCode(badgeId, out var badge) && badge.CanTrade;

    public static bool CanDeleteBadge(string badgeId) => TryGetBadgeByCode(badgeId, out var badge) && badge.CanDelete;

    public static int AmountWinwinsBadge(string badgeId) => TryGetBadgeByCode(badgeId, out var badge) ? badge.AmountWinwins : 0;

    public static List<Badge> Badges => [.. BadgeList.Values];
}
