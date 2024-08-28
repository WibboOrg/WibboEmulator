namespace WibboEmulator.Games.Badges;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class BadgeManager
{
    private static readonly string ACHIVEMENT_BADGE = "ACH_";
    private static readonly Dictionary<string, Badge> BadgeList = [];

    public static void Initialize(IDbConnection dbClient)
    {
        BadgeList.Clear();

        var emulatorBadgeList = EmulatorBadgeDao.GetAll(dbClient);

        foreach (var emulatorBadge in emulatorBadgeList)
        {
            if (!BadgeList.ContainsKey(emulatorBadge.Code))
            {
                BadgeList.Add(emulatorBadge.Code, new Badge(emulatorBadge.Code, emulatorBadge.CanTrade, emulatorBadge.CanDelete, emulatorBadge.CanGive, emulatorBadge.AmountWinwins));
            }
        }

        BadgeList.Add("", new Badge("", false, true, true, 0));
    }

    public static bool IsAchivementBadge(string code) => code.StartsWith(ACHIVEMENT_BADGE);

    public static bool TryGetBadgeByCode(string code, out Badge badge) => BadgeList.TryGetValue(code, out badge) || BadgeList.TryGetValue("", out badge);

    public static bool CanGiveBadge(string code) => TryGetBadgeByCode(code, out var badge) && badge.CanGive && !IsAchivementBadge(code);

    public static bool CanTradeBadge(string code) => TryGetBadgeByCode(code, out var badge) && badge.CanTrade && !IsAchivementBadge(code);

    public static bool CanDeleteBadge(string code) => TryGetBadgeByCode(code, out var badge) && badge.CanDelete && !IsAchivementBadge(code);

    public static int AmountWinwinsBadge(string code) => TryGetBadgeByCode(code, out var badge) ? badge.AmountWinwins : 0;

    public static List<Badge> Badges => [.. BadgeList.Values];
}
