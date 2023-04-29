namespace WibboEmulator.Games.Users.Premium;

using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;

public class PremiumComponent : IDisposable
{
    private readonly User _userInstance;
    private bool _hasEverBeenMember;
    private DateTime _activated;
    private DateTime _expireClassic;
    private DateTime _expireEpic;
    private DateTime _expireLegend;

    public PremiumComponent(User user)
    {
        this._userInstance = user;

        this._activated = DateTime.UtcNow;
        this._expireClassic = DateTime.UtcNow;
        this._expireEpic = DateTime.UtcNow;
        this._expireLegend = DateTime.UtcNow;
    }

    public void Init(IQueryAdapter dbClient)
    {
        var premiumData = UserPremiumDao.GetOne(dbClient, this._userInstance.Id);

        if (premiumData != null)
        {
            this._activated = DateTime.UnixEpoch.AddSeconds(Convert.ToInt32(premiumData["timestamp_activated"]));
            this._expireClassic = DateTime.UnixEpoch.AddSeconds(Convert.ToInt32(premiumData["timestamp_expire_classic"]));
            this._expireEpic = DateTime.UnixEpoch.AddSeconds(Convert.ToInt32(premiumData["timestamp_expire_epic"]));
            this._expireLegend = DateTime.UnixEpoch.AddSeconds(Convert.ToInt32(premiumData["timestamp_expire_legend"]));

            this._hasEverBeenMember = true;
        }

        this.CheckPremiumTimeout(dbClient);
    }

    public void CheckPremiumTimeout(IQueryAdapter dbClient = null)
    {
        if (this.IsPremiumLegend == false && this._userInstance.BadgeComponent.HasBadge("WC_LEGEND"))
        {
            this._userInstance.BadgeComponent.RemoveBadge("WC_LEGEND");
        }
        else if (this.IsPremiumLegend && this._userInstance.BadgeComponent.HasBadge("WC_LEGEND") == false)
        {
            this._userInstance.BadgeComponent.GiveBadge("WC_LEGEND", true);
        }

        if (this.IsPremiumEpic == false && this._userInstance.BadgeComponent.HasBadge("WC_EPIC"))
        {
            this._userInstance.BadgeComponent.RemoveBadge("WC_EPIC");
        }
        else if (this.IsPremiumEpic && this._userInstance.BadgeComponent.HasBadge("WC_EPIC") == false)
        {
            this._userInstance.BadgeComponent.GiveBadge("WC_EPIC", true);
        }

        if (this.IsPremiumClassic == false && this._userInstance.BadgeComponent.HasBadge("WC_CLASSIC"))
        {
            this._userInstance.BadgeComponent.RemoveBadge("WC_CLASSIC");
        }
        else if (this.IsPremiumClassic && this._userInstance.BadgeComponent.HasBadge("WC_CLASSIC") == false)
        {
            this._userInstance.BadgeComponent.GiveBadge("WC_CLASSIC", true);
        }

        if (this.IsPremiumLegend == false && this.IsPremiumEpic == false && this.IsPremiumClassic == false)
        {
            if (this._userInstance.Rank == 2)
            {
                this._userInstance.Rank = 1;

                dbClient ??= WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateRank(dbClient, this._userInstance.Id, 1);
            }
        }
        else if (this.IsPremiumLegend || this.IsPremiumEpic || this.IsPremiumClassic)
        {
            if (this._userInstance.Rank == 1)
            {
                this._userInstance.Rank = 2;

                dbClient ??= WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateRank(dbClient, this._userInstance.Id, 2);
            }
        }
    }

    public void AddPremiumDays(IQueryAdapter dbClient, int days, int clubLevel)
    {
        var now = WibboEnvironment.GetUnixTimestamp();

        UserDao.UpdateAddMonthPremium(dbClient, this._userInstance.Id);

        if (this._hasEverBeenMember == false)
        {
            this._hasEverBeenMember = true;
            this._activated = DateTime.UnixEpoch.AddSeconds(now);
            UserPremiumDao.Insert(dbClient, this._userInstance.Id);
        }

        if (this.IsPremiumLegend == false)
        {
            this._expireLegend = DateTime.UtcNow;
        }
        if (this.IsPremiumEpic == false)
        {
            this._expireEpic = DateTime.UtcNow;
        }
        if (this.IsPremiumClassic == false)
        {
            this._expireClassic = DateTime.UtcNow;
        }

        if (clubLevel is 3)
        {
            this._expireLegend += TimeSpan.FromDays(days);
        }
        if (clubLevel is 3 or 2)
        {
            this._expireEpic += TimeSpan.FromDays(days);
        }
        if (clubLevel is 3 or 2 or 1)
        {
            this._expireClassic += TimeSpan.FromDays(days);
        }

        var activated = (int)((DateTimeOffset)this._activated).ToUnixTimeSeconds();
        var expireClassic = (int)((DateTimeOffset)this._expireClassic).ToUnixTimeSeconds();
        var expireEpic = (int)((DateTimeOffset)this._expireEpic).ToUnixTimeSeconds();
        var expireLegend = (int)((DateTimeOffset)this._expireLegend).ToUnixTimeSeconds();

        UserPremiumDao.UpdateExpired(dbClient, this._userInstance.Id, activated, expireClassic, expireEpic, expireLegend);

        if (this._userInstance.Rank == 1)
        {
            this._userInstance.Rank = 2;
            UserDao.UpdateRank(dbClient, this._userInstance.Id, 2);
        }
    }

    public void SendPackets(bool isLogin = false)
    {
        this._userInstance.Client.SendPacket(new ScrSendUserInfoComposer(this.ExpireTime(), isLogin, this._hasEverBeenMember));
        this._userInstance.Client.SendPacket(new ScrSendKickbackInfoComposer(this._activated));
    }

    public bool IsPremiumClassic => (this._expireClassic - DateTimeOffset.UtcNow).TotalSeconds > 0;

    public bool IsPremiumEpic => (this._expireEpic - DateTimeOffset.UtcNow).TotalSeconds > 0;

    public bool IsPremiumLegend => (this._expireLegend - DateTimeOffset.UtcNow).TotalSeconds > 0;

    public TimeSpan ExpireTime()
    {
        var timeSpans = new List<TimeSpan>()
        {
            this._expireClassic - DateTimeOffset.UtcNow,
            this._expireEpic - DateTimeOffset.UtcNow,
            this._expireLegend - DateTimeOffset.UtcNow
        };

        var maxTimeSpan = TimeSpan.MinValue;

        foreach (var timeSpan in timeSpans)
        {
            if (timeSpan > maxTimeSpan)
            {
                maxTimeSpan = timeSpan;
            }
        }

        return maxTimeSpan;
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
