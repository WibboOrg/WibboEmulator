namespace WibboEmulator.Games.Users.Badges;
using System.Collections;
using System.Data;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Users;

public class BadgeComponent : IDisposable
{
    private readonly User _userInstance;

    public BadgeComponent(User user)
    {
        this._userInstance = user;
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
            var num = 0;
            foreach (Badge badge in (IEnumerable)this.BadgeList.Values)
            {
                if (badge.Slot == 0)
                {
                    continue;
                }

                num++;
            }

            return (num > 5) ? 5 : num;
        }
    }

    public Dictionary<string, Badge> BadgeList { get; }

    public bool HasBadgeSlot(string badge)
    {
        if (this.BadgeList.ContainsKey(badge))
        {
            return this.BadgeList[badge].Slot > 0;
        }
        else
        {
            return false;
        }
    }

    public ICollection<Badge> GetBadges() => this.BadgeList.Values;

    public Badge GetBadge(string badge)
    {
        if (this.BadgeList.ContainsKey(badge))
        {
            return this.BadgeList[badge];
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

    public void Dispose()
    {
        this.BadgeList.Clear();
        GC.SuppressFinalize(this);
    }
}
