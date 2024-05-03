namespace WibboEmulator.Games.Users.Banners;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Users;

public class BannerComponent(User user) : IDisposable
{
    public List<Banner> BannerList { get; } = [];

    public void Initialize(IDbConnection dbClient)
    {
        this.LoadDefaultBanner();

        var emulatorBannerIdList = UserBannerDao.GetAll(dbClient, user.Id);

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
        if (user.HasPermission("premium_legend"))
        {
            this.LoadBanner(5);
            this.LoadBanner(6);
            this.LoadBanner(7);
        }

        if (user.HasPermission("premium_epic"))
        {
            this.LoadBanner(4);
            this.LoadBanner(3);
        }

        if (user.HasPermission("premium_classic"))
        {
            this.LoadBanner(2);
        }

        this.LoadBanner(1);
        this.LoadBanner(73);

        if (user.HasPermission("banner_all"))
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

            UserBannerDao.Insert(dbClient, user.Id, id);

            user.Client?.SendPacket(new UnseenItemsComposer(id, UnseenItemsType.Banner));
            user.Client?.SendPacket(new UserBannerListComposer(this.BannerList));
        }
    }

    public void RemoveBanner(IDbConnection dbClient, int id)
    {
        if (BannerManager.TryGetBannerById(id, out var banner) && this.BannerList.Contains(banner))
        {
            _ = this.BannerList.Remove(banner);

            UserBannerDao.Delete(dbClient, user.Id, id);

            if (user.BannerSelected == banner)
            {
                user.BannerSelected = null;
            }

            user.Client?.SendPacket(new UserBannerListComposer(this.BannerList));
        }
    }

    public void Dispose()
    {
        this.BannerList.Clear();
        GC.SuppressFinalize(this);
    }
}
