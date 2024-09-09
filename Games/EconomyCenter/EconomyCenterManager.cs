namespace WibboEmulator.Games.Banners;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class EconomyCenterManager
{
    private static readonly Dictionary<int, EmulatorEconomyEntity> EconomyItemList = [];

    public static void Initialize(IDbConnection dbClient)
    {
        EconomyItemList.Clear();

        var emulatorEconomyItemList = EmulatorEconomyDao.GetAll(dbClient);

        foreach (var economyItem in emulatorEconomyItemList)
        {
            EconomyItemList.Add(economyItem.Id, economyItem);
        }
    }

    public static List<EmulatorEconomyEntity> EconomyItem => [.. EconomyItemList.Values];
}
