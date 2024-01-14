namespace WibboEmulator.Games.Users.Badges;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Users;

public class BannerComponent : IDisposable
{
    private readonly User _userInstance;
    public List<Banner> BannerList { get; }

    public BannerComponent(User user)
    {
        this._userInstance = user;
        this.BannerList = new();
    }

    public void Init(IDbConnection dbClient)
    {
        this.LoadDefaultBanner();

        var emulatorBannerIdList = UserBannerDao.GetAll(dbClient, this._userInstance.Id);

        foreach (var id in emulatorBannerIdList)
        {
            var banner = WibboEnvironment.GetGame().GetBannerManager().GetBannerById(id);

            if (!this.BannerList.Contains(banner))
            {
                this.BannerList.Add(banner);
            }
        }
    }

    public void LoadDefaultBanner()
    {
        if (this._userInstance.HasPermission("premium_legend"))
        {
            this.LoadBanner(5);
            this.LoadBanner(6);
            this.LoadBanner(7);
        }

        if (this._userInstance.HasPermission("premium_epic"))
        {
            this.LoadBanner(4);
            this.LoadBanner(3);
        }

        if (this._userInstance.HasPermission("premium_classic"))
        {
            this.LoadBanner(2);
        }

        this.LoadBanner(1);
        this.LoadBanner(73);

        if (this._userInstance.HasPermission("banner_all"))
        {
            foreach (var banner in WibboEnvironment.GetGame().GetBannerManager().GetBanners())
            {
                if (!this.BannerList.Contains(banner))
                {
                    this.BannerList.Add(banner);
                }
            }
        }
    }

    private void LoadBanner(int id)
    {
        var banner = WibboEnvironment.GetGame().GetBannerManager().GetBannerById(id);

        if (banner != null && !this.BannerList.Contains(banner))
        {
            this.BannerList.Add(banner);
        }
    }

    public void AddBanner(IDbConnection dbClient, int id)
    {
        var banner = WibboEnvironment.GetGame().GetBannerManager().GetBannerById(id);

        if (banner != null && !this.BannerList.Contains(banner))
        {
            this.BannerList.Add(banner);

            UserBannerDao.Insert(dbClient, this._userInstance.Id, id);

            this._userInstance.Client?.SendPacket(new UnseenItemsComposer(id, UnseenItemsType.Banner));
            this._userInstance.Client?.SendPacket(new UserBannerListComposer(this.BannerList));
        }
    }

    public void RemoveBanner(IDbConnection dbClient, int id)
    {
        var banner = WibboEnvironment.GetGame().GetBannerManager().GetBannerById(id);

        if (this.BannerList.Contains(banner))
        {
            this.BannerList.Remove(banner);

            UserBannerDao.Delete(dbClient, this._userInstance.Id, id);

            if (this._userInstance.BannerSelected == banner)
            {
                this._userInstance.BannerSelected = null;
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
