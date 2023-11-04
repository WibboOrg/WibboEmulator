namespace WibboEmulator.Games.Users.Badges;
using System.Collections;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Users;

public class BadgeComponent : IDisposable
{
    private readonly User _userInstance;
    private readonly int _maxBadgeCount;

    private int _virutalBadgeId;

    public BadgeComponent(User user)
    {
        this._userInstance = user;
        this._maxBadgeCount = WibboEnvironment.GetSettings().GetData<int>("badge.max.count");
        this._virutalBadgeId = 0;
        this.BadgeList = new Dictionary<string, Badge>();
    }

    public void Init(IQueryAdapter dbClient)
    {
        var table = UserBadgeDao.GetAll(dbClient, this._userInstance.Id);

        foreach (DataRow dataRow in table.Rows)
        {
            var code = (string)dataRow["badge_id"];
            var slot = Convert.ToInt32(dataRow["badge_slot"]);

            if (!this.BadgeList.ContainsKey(code))
            {
                this.BadgeList.Add(code, new Badge(code, slot));
            }
        }
    }

    public int EquippedCount
    {
        get
        {
            var count = 0;
            foreach (var badge in this.BadgeList.Values)
            {
                if (badge.Slot == 0)
                {
                    continue;
                }

                count++;
            }

            return (count > this._maxBadgeCount) ? this._maxBadgeCount : count;
        }
    }

    public Dictionary<string, Badge> BadgeList { get; }

    public bool HasBadgeSlot(string badge)
    {
        if (this.BadgeList.TryGetValue(badge, out var value))
        {
            return value.Slot > 0;
        }
        else
        {
            return false;
        }
    }

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

    public void GiveBadge(string badge, bool inDatabase) => this.GiveBadge(badge, 0, inDatabase);

    public void GiveBadge(string badge, int slot, bool inDatabase)
    {
        if (this.HasBadge(badge))
        {
            return;
        }

        if (inDatabase)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserBadgeDao.Insert(dbClient, this._userInstance.Id, slot, badge);
        }

        this.BadgeList.Add(badge, new Badge(badge, slot));

        this._virutalBadgeId++;

        this._userInstance.Client?.SendPacket(new UnseenItemsComposer(this._virutalBadgeId, UnseenItemsType.Badge));
        this._userInstance.Client?.SendPacket(new ReceiveBadgeComposer(this._virutalBadgeId, badge));
    }

    public void ResetSlots()
    {
        foreach (Badge badge in (IEnumerable)this.BadgeList.Values)
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

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserBadgeDao.Delete(dbClient, this._userInstance.Id, badge);
        }

        _ = this.BadgeList.Remove(this.GetBadge(badge).Code);
    }

    public int GetEmblemId()
    {
        var emblems = new Dictionary<string, int>
        {
            { "STAFF_GESTION", 587 },
            { "STAFF_DEV", 606 },
            { "STAFF_FURNI", 595 },
            { "STAFF_ADMIN", 583 },
            { "STAFF_ANIMATEUR", 584 },
            { "STAFF_EVENT", 594 },
            { "STAFF_MODO", 589 },
            { "STAFF_ARCHITECTE", 585 },
            { "STAFF_CASINO", 586 },
            { "STAFF_GRAPH", 588 },
            { "STAFF_PROWIRED", 590 },
            { "ADM", 102 },
            { "PRWRD1", 580 },
            { "GPHWIB", 557 },
            { "wibbo.helpeur", 544 },
            { "WIBARC", 546 },
            { "CRPOFFI", 570 },
            { "ZEERSWS", 552 },
            { "WBASSO", 576 },
            { "WIBBOCOM", 581 },
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
            { "ADM", 23 },
            { "STAFF_GESTION", 53 },
            { "STAFF_DEV", 41 },
            { "STAFF_ADMIN", 41 },
            { "STAFF_FURNI", 41 },
            { "STAFF_ANIMATEUR", 42 },
            { "STAFF_EVENT", 52 },
            { "STAFF_MODO", 47 },
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
