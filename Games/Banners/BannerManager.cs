namespace WibboEmulator.Games.Banners;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Utilities;

public static class BannerManager
{
    private static readonly Dictionary<int, Banner> BannerList = [];

    public static void Initialize(IDbConnection dbClient)
    {
        BannerList.Clear();

        var emulatorBannerList = EmulatorBannerDao.GetAll(dbClient);

        foreach (var emulatorBanner in emulatorBannerList)
        {
            BannerList.Add(emulatorBanner.Id, new Banner(emulatorBanner.Id, emulatorBanner.HaveLayer, emulatorBanner.CanTrade));
        }
    }

    public static Banner GetOneRandomBanner()
    {
        if (BannerList.Count == 0)
        {
            return null;
        }

        var addBannerLayer = WibboEnvironment.GetRandomNumber(0, 2) == 2;

        var allBannerTrade = BannerList.Values.Where(x => (x.CanTrade && !x.HaveLayer) || (x.CanTrade && x.HaveLayer && addBannerLayer)).ToList();

        if (allBannerTrade.Count == 0)
        {
            return null;
        }

        return allBannerTrade.GetRandomElement();
    }

    public static List<Banner> Banners => [.. BannerList.Values];

    public static bool TryGetBannerById(int id, out Banner banner) => BannerList.TryGetValue(id, out banner);
}
