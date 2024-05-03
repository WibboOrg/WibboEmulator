namespace WibboEmulator.Games.Users.Premium;

using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using System.Data;
using WibboEmulator.Database;

public class PremiumComponent(User user) : IDisposable
{
    private bool _hasEverBeenMember;
    private DateTime _activated = DateTime.UtcNow;
    private DateTime _expireClassic = DateTime.UtcNow;
    private DateTime _expireEpic = DateTime.UtcNow;
    private DateTime _expireLegend = DateTime.UtcNow;

    public void Initialize(IDbConnection dbClient)
    {
        var userPremium = UserPremiumDao.GetOne(dbClient, user.Id);

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
        if (this.IsPremiumLegend == false && user.BadgeComponent.HasBadge("WC_LEGEND"))
        {
            user.BadgeComponent.RemoveBadge("WC_LEGEND");
        }
        else if (this.IsPremiumLegend && user.BadgeComponent.HasBadge("WC_LEGEND") == false)
        {
            user.BadgeComponent.GiveBadge("WC_LEGEND");
        }

        if (this.IsPremiumEpic == false && user.BadgeComponent.HasBadge("WC_EPIC"))
        {
            user.BadgeComponent.RemoveBadge("WC_EPIC");
        }
        else if (this.IsPremiumEpic && user.BadgeComponent.HasBadge("WC_EPIC") == false)
        {
            user.BadgeComponent.GiveBadge("WC_EPIC");
        }

        if (this.IsPremiumClassic == false && user.BadgeComponent.HasBadge("WC_CLASSIC"))
        {
            user.BadgeComponent.RemoveBadge("WC_CLASSIC");
        }
        else if (this.IsPremiumClassic && user.BadgeComponent.HasBadge("WC_CLASSIC") == false)
        {
            user.BadgeComponent.GiveBadge("WC_CLASSIC");
        }

        if (this.IsPremiumLegend == false && this.IsPremiumEpic == false && this.IsPremiumClassic == false)
        {
            if (user.Rank == 2)
            {
                user.Rank = 1;

                dbClient ??= DatabaseManager.Connection;
                UserDao.UpdateRank(dbClient, user.Id, 1);
            }
        }
        else if (this.IsPremiumLegend || this.IsPremiumEpic || this.IsPremiumClassic)
        {
            if (user.Rank == 1)
            {
                user.Rank = 2;

                dbClient ??= DatabaseManager.Connection;
                UserDao.UpdateRank(dbClient, user.Id, 2);
            }
        }
    }

    public void AddPremiumDays(IDbConnection dbClient, int days, PremiumClubLevel clubLevel)
    {
        var now = WibboEnvironment.GetUnixTimestamp();

        UserDao.UpdateAddMonthPremium(dbClient, user.Id);

        if (this._hasEverBeenMember == false)
        {
            this._hasEverBeenMember = true;
            this._activated = DateTime.UnixEpoch.AddSeconds(now);
            UserPremiumDao.Insert(dbClient, user.Id);
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

        UserPremiumDao.UpdateExpired(dbClient, user.Id, activated, expireClassic, expireEpic, expireLegend);

        if (user.Rank == 1)
        {
            user.Rank = 2;
            UserDao.UpdateRank(dbClient, user.Id, 2);

            user.Client.SendPacket(new UserRightsComposer(user.Rank < 2 ? 2 : user.Rank, user.Rank > 1));
        }
    }

    public void SendPackets(bool isLogin = false)
    {
        user.Client.SendPacket(new ScrSendUserInfoComposer(this.ExpireTime(), isLogin, this._hasEverBeenMember));
        user.Client.SendPacket(new ScrSendKickbackInfoComposer(this._activated));
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
