namespace WibboEmulator.Games.Users.Badges;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Users;

public class BadgeComponent : IDisposable
{
    private readonly User _userInstance;
    private readonly int _maxBadgeCount;

    private int _virtualBadgeId;

    public BadgeComponent(User user)
    {
        this._userInstance = user;
        this._maxBadgeCount = WibboEnvironment.GetSettings().GetData<int>("badge.max.count");
        this._virtualBadgeId = 0;
        this.BadgeList = new Dictionary<string, Badge>();
    }

    public void Initialize(IDbConnection dbClient)
    {
        var userBadgeList = UserBadgeDao.GetAll(dbClient, this._userInstance.Id);

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

    public Dictionary<string, Badge> BadgeList { get; }

    public bool HasBadgeSlot(string badge) => this.BadgeList.TryGetValue(badge, out var value) && value.Slot > 0;

    public ICollection<Badge> GetBadges() => this.BadgeList.Values;

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
            using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
            UserBadgeDao.Insert(dbClient, this._userInstance.Id, 0, badge);
        }

        this.BadgeList.Add(badge, new Badge(badge, 0));

        this._virtualBadgeId++;

        this._userInstance.Client?.SendPacket(new UnseenItemsComposer(this._virtualBadgeId, UnseenItemsType.Badge));
        this._userInstance.Client?.SendPacket(new ReceiveBadgeComposer(this._virtualBadgeId, badge));
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

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            UserBadgeDao.Delete(dbClient, this._userInstance.Id, badge);
        }

        _ = this.BadgeList.Remove(badge);
        this._userInstance.Client?.SendPacket(new RemovedBadgeComposer(badge));
    }

    public int GetEmblemId()
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

    public int GetStaffBulleId()
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

    public int BadgeMaxCount() => this._maxBadgeCount;

    public void Dispose()
    {
        this.BadgeList.Clear();
        GC.SuppressFinalize(this);
    }
}
