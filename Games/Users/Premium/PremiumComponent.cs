namespace WibboEmulator.Games.Users.Premium;

using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using System.Data;

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

    public void Initialize(IDbConnection dbClient)
    {
        var userPremium = UserPremiumDao.GetOne(dbClient, this._userInstance.Id);

        if (userPremium != null)
        {
            this._activated = DateTime.UnixEpoch.AddSeconds(userPremium.TimestampActivated);
            this._expireClassic = DateTime.UnixEpoch.AddSeconds(userPremium.TimestampExpireClassic);
            this._expireEpic = DateTime.UnixEpoch.AddSeconds(userPremium.TimestampExpireEpic);
            this._expireLegend = DateTime.UnixEpoch.AddSeconds(userPremium.TimestampExpireLegend);

            this._hasEverBeenMember = true;
        }

        this.CheckPremiumTimeout(dbClient);
    }

    public void CheckPremiumTimeout(IDbConnection dbClient = null)
    {
        if (this.IsPremiumLegend == false && this._userInstance.BadgeComponent.HasBadge("WC_LEGEND"))
        {
            this._userInstance.BadgeComponent.RemoveBadge("WC_LEGEND");
        }
        else if (this.IsPremiumLegend && this._userInstance.BadgeComponent.HasBadge("WC_LEGEND") == false)
        {
            this._userInstance.BadgeComponent.GiveBadge("WC_LEGEND");
        }

        if (this.IsPremiumEpic == false && this._userInstance.BadgeComponent.HasBadge("WC_EPIC"))
        {
            this._userInstance.BadgeComponent.RemoveBadge("WC_EPIC");
        }
        else if (this.IsPremiumEpic && this._userInstance.BadgeComponent.HasBadge("WC_EPIC") == false)
        {
            this._userInstance.BadgeComponent.GiveBadge("WC_EPIC");
        }

        if (this.IsPremiumClassic == false && this._userInstance.BadgeComponent.HasBadge("WC_CLASSIC"))
        {
            this._userInstance.BadgeComponent.RemoveBadge("WC_CLASSIC");
        }
        else if (this.IsPremiumClassic && this._userInstance.BadgeComponent.HasBadge("WC_CLASSIC") == false)
        {
            this._userInstance.BadgeComponent.GiveBadge("WC_CLASSIC");
        }

        if (this.IsPremiumLegend == false && this.IsPremiumEpic == false && this.IsPremiumClassic == false)
        {
            if (this._userInstance.Rank == 2)
            {
                this._userInstance.Rank = 1;

                dbClient ??= WibboEnvironment.GetDatabaseManager().Connection();
                UserDao.UpdateRank(dbClient, this._userInstance.Id, 1);
            }
        }
        else if (this.IsPremiumLegend || this.IsPremiumEpic || this.IsPremiumClassic)
        {
            if (this._userInstance.Rank == 1)
            {
                this._userInstance.Rank = 2;

                dbClient ??= WibboEnvironment.GetDatabaseManager().Connection();
                UserDao.UpdateRank(dbClient, this._userInstance.Id, 2);
            }
        }
    }

    public void AddPremiumDays(IDbConnection dbClient, int days, PremiumClubLevel clubLevel)
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

        if (clubLevel is PremiumClubLevel.LEGEND)
        {
            this._expireLegend += TimeSpan.FromDays(days);
        }
        if (clubLevel is PremiumClubLevel.LEGEND or PremiumClubLevel.EPIC)
        {
            this._expireEpic += TimeSpan.FromDays(days);
        }
        if (clubLevel is PremiumClubLevel.LEGEND or PremiumClubLevel.EPIC or PremiumClubLevel.CLASSIC)
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

            this._userInstance.Client.SendPacket(new UserRightsComposer(this._userInstance.Rank < 2 ? 2 : this._userInstance.Rank, this._userInstance.Rank > 1));
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

public enum PremiumClubLevel
{
    CLASSIC,
    EPIC,
    LEGEND
}
