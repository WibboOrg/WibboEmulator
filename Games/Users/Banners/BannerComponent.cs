namespace WibboEmulator.Games.Users.Badges;
using System.Data;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Users;

public class BannerComponent : IDisposable
{
    private readonly User _userInstance;
    private readonly List<int> _bannerList;

    public BannerComponent(User user)
    {
        this._userInstance = user;
        this._bannerList = new();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this.LoadDefaultBanner();

        var table = UserBannerDao.GetAll(dbClient, this._userInstance.Id);

        foreach (DataRow dataRow in table.Rows)
        {
            var id = Convert.ToInt32(dataRow["banner_id"]);

            if (!this._bannerList.Contains(id))
            {
                this._bannerList.Add(id);
            }
        }
    }

    public void LoadDefaultBanner()
    {
        if (this._userInstance.Premium != null)
        {
            if (this._userInstance.Premium.IsPremiumLegend)
            {
                this._bannerList.Add(5);
                this._bannerList.Add(6);
                this._bannerList.Add(7);
            }

            if (this._userInstance.Premium.IsPremiumEpic)
            {
                this._bannerList.Add(4);
                this._bannerList.Add(3);
            }

            if (this._userInstance.Premium.IsPremiumClassic)
            {
                this._bannerList.Add(2);
            }
        }

        this._bannerList.Add(1);
        this._bannerList.Add(0);
    }

    public void AddBanner(IQueryAdapter dbClient, int id)
    {
        if (!this._bannerList.Contains(id))
        {
            this._bannerList.Add(id);

            UserBannerDao.Insert(dbClient, this._userInstance.Id, id);
        }
    }

    public void RemoveBanner(IQueryAdapter dbClient, int id)
    {
        if (this._bannerList.Contains(id))
        {
            this._bannerList.Remove(id);

            UserBannerDao.Delete(dbClient, this._userInstance.Id, id);
        }
    }

    public List<int> BannerList => this._bannerList;

    public void Dispose()
    {
        this._bannerList.Clear();
        GC.SuppressFinalize(this);
    }
}
