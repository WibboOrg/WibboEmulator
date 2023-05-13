namespace WibboEmulator.Games.Users.Badges;
using System.Data;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Users;

public class BannerComponent : IDisposable
{
    private readonly User _userInstance;
    public List<int> BannerList { get; }

    public BannerComponent(User user)
    {
        this._userInstance = user;
        this.BannerList = new();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this.LoadDefaultBanner();

        var table = UserBannerDao.GetAll(dbClient, this._userInstance.Id);

        foreach (DataRow dataRow in table.Rows)
        {
            var id = Convert.ToInt32(dataRow["banner_id"]);

            if (!this.BannerList.Contains(id))
            {
                this.BannerList.Add(id);
            }
        }
    }

    public void LoadDefaultBanner()
    {
        if (this._userInstance.HasPermission("premium_legend"))
        {
            this.BannerList.Add(5);
            this.BannerList.Add(6);
            this.BannerList.Add(7);
        }

        if (this._userInstance.HasPermission("premium_epic"))
        {
            this.BannerList.Add(4);
            this.BannerList.Add(3);
        }

        if (this._userInstance.HasPermission("premium_classic"))
        {
            this.BannerList.Add(2);
        }

        this.BannerList.Add(1);
        this.BannerList.Add(0);
    }

    public void AddBanner(IQueryAdapter dbClient, int id)
    {
        if (!this.BannerList.Contains(id))
        {
            this.BannerList.Add(id);

            UserBannerDao.Insert(dbClient, this._userInstance.Id, id);
        }
    }

    public void RemoveBanner(IQueryAdapter dbClient, int id)
    {
        if (this.BannerList.Contains(id))
        {
            this.BannerList.Remove(id);

            UserBannerDao.Delete(dbClient, this._userInstance.Id, id);
        }
    }

    public void Dispose()
    {
        this.BannerList.Clear();
        GC.SuppressFinalize(this);
    }
}
