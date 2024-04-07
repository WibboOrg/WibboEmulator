namespace WibboEmulator.Games.Banners;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public class BannerManager
{
    private readonly Dictionary<int, Banner> _banners;

    public BannerManager() => this._banners = new Dictionary<int, Banner>();

    public void Initialize(IDbConnection dbClient)
    {
        this._banners.Clear();

        var emulatorBannerList = EmulatorBannerDao.GetAll(dbClient);

        if (!emulatorBannerList.Any())
        {
            return;
        }

        foreach (var emulatorBanner in emulatorBannerList)
        {
            this._banners.Add(emulatorBanner.Id, new Banner(emulatorBanner.Id, emulatorBanner.HaveLayer, emulatorBanner.CanTrade));
        }
    }

    public Banner GetOneRandomBanner()
    {
        if (!this._banners.Any())
        {
            return null;
        }

        var allBannerTrade = this._banners.Values.Where(x => x.CanTrade);

        if (!allBannerTrade.Any())
        {
            return null;
        }

        return allBannerTrade.ElementAt(WibboEnvironment.GetRandomNumber(0, allBannerTrade.Count() - 1));
    }

    public List<Banner> GetBanners() => this._banners.Values.ToList();

    public bool TryGetBannerById(int id, out Banner banner) => this._banners.TryGetValue(id, out banner);
}
