namespace WibboEmulator.Games.Banners;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class EconomyCenterManager
{
    private static readonly Dictionary<int, EmulatorEconomyEntity> EconomyItemList = [];
    private static readonly Dictionary<int, EmulatorEconomyCategoryEntity> EconomyCategoryList = [];

    public static void Initialize(IDbConnection dbClient)
    {
        EconomyItemList.Clear();
        EconomyCategoryList.Clear();

        var emulatorEconomyItemList = EmulatorEconomyDao.GetAll(dbClient);

        foreach (var economyItem in emulatorEconomyItemList)
        {
            EconomyItemList.Add(economyItem.Id, economyItem);
        }

        var emulatorEconomyCategoryList = EmulatorEconomyCategoryDao.GetAll(dbClient);

        foreach (var economyCategory in emulatorEconomyCategoryList)
        {
            EconomyCategoryList.Add(economyCategory.Id, economyCategory);
        }
    }

    public static List<EmulatorEconomyEntity> EconomyItem => [.. EconomyItemList.Values];

    public static List<EmulatorEconomyCategoryEntity> EconomyCategory => [.. EconomyCategoryList.Values];
}
