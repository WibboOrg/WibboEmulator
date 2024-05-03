namespace WibboEmulator.Games.Users.Badges;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Users;

public class BadgeComponent(User user) : IDisposable
{
    private int _virtualBadgeId;

    public Dictionary<string, Badge> BadgeList { get; } = [];

    public void Initialize(IDbConnection dbClient, bool onlyProfil = false)
    {
        var userBadgeList = onlyProfil ? UserBadgeDao.GetAllProfil(dbClient, user.Id) : UserBadgeDao.GetAll(dbClient, user.Id);

        foreach (var userBadge in userBadgeList)
        {
            var code = userBadge.BadgeId;
            var slot = userBadge.BadgeSlot;

            if (!this.BadgeList.ContainsKey(code))
            {
                this.BadgeList.Add(code, new Badge(code, slot));
            }
        }
    }

    public bool HasBadgeSlot(string badge) => this.BadgeList.TryGetValue(badge, out var value) && value.Slot > 0;

    public ICollection<Badge> Badges => this.BadgeList.Values;

    public Badge GetBadge(string badge)
    {
        if (this.BadgeList.TryGetValue(badge, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public bool HasBadge(string badge)
    {
        if (string.IsNullOrEmpty(badge))
        {
            return true;
        }

        return this.BadgeList.ContainsKey(badge);
    }

    public void GiveBadge(string badge, bool inDatabase = true)
    {
        if (this.HasBadge(badge))
        {
            return;
        }

        if (inDatabase)
        {
            using var dbClient = DatabaseManager.Connection;
            UserBadgeDao.Insert(dbClient, user.Id, 0, badge);
        }

        this.BadgeList.Add(badge, new Badge(badge, 0));

        this._virtualBadgeId++;

        user.Client?.SendPacket(new UnseenItemsComposer(this._virtualBadgeId, UnseenItemsType.Badge));
        user.Client?.SendPacket(new ReceiveBadgeComposer(this._virtualBadgeId, badge));
    }

    public void ResetSlots()
    {
        foreach (var badge in this.BadgeList.Values)
        {
            badge.Slot = 0;
        }
    }

    public void RemoveBadge(string badge)
    {
        if (!this.HasBadge(badge))
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserBadgeDao.Delete(dbClient, user.Id, badge);
        }

        _ = this.BadgeList.Remove(badge);
        user.Client?.SendPacket(new RemovedBadgeComposer(badge));
    }

    public int EmblemId
    {
        get
        {
            var emblems = new Dictionary<string, int>
        {
            { "STAFF_GESTION", 587 },
            { "STAFF_DEV", 606 },
            { "STAFF_FURNI", 595 },
            { "STAFF_ADMIN", 583 },
            { "STAFF_COM", 781 },
            { "STAFF_ANIMATEUR", 584 },
            { "STAFF_EVENT", 594 },
            { "STAFF_MODO", 589 },
            { "STAFF_HELPER", 648 },
            { "STAFF_ARCHITECTE", 585 },
            { "STAFF_CASINO", 586 },
            { "STAFF_GRAPH", 588 },
            { "STAFF_PROWIRED", 590 },
            { "WC_LEGEND", 593 },
            { "WC_EPIC", 592 },
            { "WC_CLASSIC", 591 },
            { "WIBORGOFF", 607 },
        };

            var enableId = 0;

            foreach (var emblem in emblems)
            {
                if (this.HasBadgeSlot(emblem.Key))
                {
                    enableId = emblem.Value;
                    break;
                }
            }

            return enableId;
        }
    }

    public int StaffBulleId
    {
        get
        {
            var bubbles = new Dictionary<string, int>
        {
            { "STAFF_GESTION", 41 },
            { "STAFF_DEV", 41 },
            { "STAFF_ADMIN", 41 },
            { "STAFF_FURNI", 41 },
            { "STAFF_COMM", 41 },
            { "STAFF_ANIMATEUR", 42 },
            { "STAFF_EVENT", 52 },
            { "STAFF_MODO", 47 },
            { "STAFF_HELPER", 53 },
            { "STAFF_ARCHITECTE", 43 },
            { "STAFF_CASINO", 44 },
            { "STAFF_GRAPH", 46 },
            { "STAFF_PROWIRED", 48 },
        };

            var bubbleId = 23;

            foreach (var bubble in bubbles)
            {
                if (this.HasBadgeSlot(bubble.Key))
                {
                    bubbleId = bubble.Value;
                    break;
                }
            }

            return bubbleId;
        }
    }

    public int BadgeMaxCount { get; } = SettingsManager.GetData<int>("badge.max.count");

    public void Dispose()
    {
        this.BadgeList.Clear();
        GC.SuppressFinalize(this);
    }
}
