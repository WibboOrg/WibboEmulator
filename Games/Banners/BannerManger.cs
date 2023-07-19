namespace WibboEmulator.Games.Banners;

using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

public class BannerManager
{
    private readonly Dictionary<int, Banner> _banners;

    public BannerManager() => this._banners = new Dictionary<int, Banner>();

    public void Init(IQueryAdapter dbClient)
    {
        this._banners.Clear();

        var table = EmulatorBannerDao.GetAll(dbClient);
        foreach (DataRow dataRow in table.Rows)
        {
            var id = Convert.ToInt32(dataRow["id"]);
            var haveLayer = Convert.ToBoolean(dataRow["have_layer"]);

            this._banners.Add(id, new Banner(id, haveLayer));
        }
    }

    public List<Banner> GetBanners() => this._banners.Values.ToList();

    public Banner GetBannerById(int id) => this._banners.TryGetValue(id, out var banner) ? banner : null;
}