namespace WibboEmulator.Games.Banners;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public class BannerManager
{
    private readonly Dictionary<int, Banner> _banners;

    public BannerManager() => this._banners = new Dictionary<int, Banner>();

    public void Init(IDbConnection dbClient)
    {
        this._banners.Clear();

        var emulatorBannerList = EmulatorBannerDao.GetAll(dbClient);

        if (emulatorBannerList.Count == 0)
        {
            return;
        }

        foreach (var emulatorBanner in emulatorBannerList)
        {
            this._banners.Add(emulatorBanner.Id, new Banner(emulatorBanner.Id, emulatorBanner.HaveLayer));
        }
    }

    public List<Banner> GetBanners() => this._banners.Values.ToList();

    public Banner GetBannerById(int id) => this._banners.TryGetValue(id, out var banner) ? banner : null;
}