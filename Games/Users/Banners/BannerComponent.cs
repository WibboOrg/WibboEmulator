namespace WibboEmulator.Games.Users.Banners;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Users;

public class BannerComponent : IDisposable
{
    private readonly User _user;
    public List<Banner> BannerList { get; }

    public BannerComponent(User user)
    {
        this._user = user;
        this.BannerList = [];
    }

    public void Initialize(IDbConnection dbClient)
    {
        this.LoadDefaultBanner();

        var emulatorBannerIdList = UserBannerDao.GetAll(dbClient, this._user.Id);

        foreach (var id in emulatorBannerIdList)
        {
            if (BannerManager.TryGetBannerById(id, out var banner) && !this.BannerList.Contains(banner))
            {
                this.BannerList.Add(banner);
            }
        }
    }

    public void LoadDefaultBanner()
    {
        if (this._user.HasPermission("premium_legend"))
        {
            this.LoadBanner(5);
            this.LoadBanner(6);
            this.LoadBanner(7);
        }

        if (this._user.HasPermission("premium_epic"))
        {
            this.LoadBanner(4);
            this.LoadBanner(3);
        }

        if (this._user.HasPermission("premium_classic"))
        {
            this.LoadBanner(2);
        }

        this.LoadBanner(1);
        this.LoadBanner(73);

        if (this._user.HasPermission("banner_all"))
        {
            foreach (var banner in BannerManager.Banners)
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
        if (BannerManager.TryGetBannerById(id, out var banner) && !this.BannerList.Contains(banner))
        {
            this.BannerList.Add(banner);
        }
    }

    public void AddBanner(IDbConnection dbClient, int id)
    {
        if (BannerManager.TryGetBannerById(id, out var banner) && !this.BannerList.Contains(banner))
        {
            this.BannerList.Add(banner);

            UserBannerDao.Insert(dbClient, this._user.Id, id);

            this._user.Client?.SendPacket(new UnseenItemsComposer(id, UnseenItemsType.Banner));
            this._user.Client?.SendPacket(new UserBannerListComposer(this.BannerList));
        }
    }

    public void RemoveBanner(IDbConnection dbClient, int id)
    {
        if (BannerManager.TryGetBannerById(id, out var banner) && this.BannerList.Contains(banner))
        {
            this.BannerList.Remove(banner);

            UserBannerDao.Delete(dbClient, this._user.Id, id);

            if (this._user.BannerSelected == banner)
            {
                this._user.BannerSelected = null;
            }

            this._user.Client?.SendPacket(new UserBannerListComposer(this.BannerList));
        }
    }

    public void Dispose()
    {
        this.BannerList.Clear();
        GC.SuppressFinalize(this);
    }
}
