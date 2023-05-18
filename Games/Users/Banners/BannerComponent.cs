namespace WibboEmulator.Games.Users.Badges;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Users;

public class BannerComponent : IDisposable
{
    private readonly User _userInstance;
    private int _bannerAmount;
    public List<int> BannerList { get; }

    public BannerComponent(User user)
    {
        this._userInstance = user;
        this._bannerAmount = 0;
        this.BannerList = new();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this._bannerAmount = WibboEnvironment.GetSettings().GetData<int>("banner.amount");
        
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

        if (this._userInstance.HasPermission("banner_all"))
        {
            for (var id = 0; id < this._bannerAmount; id++)
            {
                if (!this.BannerList.Contains(id))
                {
                    this.BannerList.Add(id);
                }
            }
        }
    }

    public void AddBanner(IQueryAdapter dbClient, int id)
    {
        if (!this.BannerList.Contains(id))
        {
            this.BannerList.Add(id);

            UserBannerDao.Insert(dbClient, this._userInstance.Id, id);

            this._userInstance.Client?.SendPacket(new UnseenItemsComposer(id, UnseenItemsType.Banner));
            this._userInstance.Client?.SendPacket(new UserBannerListComposer(this.BannerList));
        }
    }

    public void RemoveBanner(IQueryAdapter dbClient, int id)
    {
        if (this.BannerList.Contains(id))
        {
            this.BannerList.Remove(id);

            UserBannerDao.Delete(dbClient, this._userInstance.Id, id);

            if (this._userInstance.BannerId == id)
            {
                this._userInstance.BannerId = -1;
            }

            this._userInstance.Client?.SendPacket(new UserBannerListComposer(this.BannerList));
        }
    }

    public void Dispose()
    {
        this.BannerList.Clear();
        GC.SuppressFinalize(this);
    }
}
