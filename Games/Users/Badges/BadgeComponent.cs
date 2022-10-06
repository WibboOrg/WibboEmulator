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
            var Code = (string)dataRow["badge_id"];
            var Slot = Convert.ToInt32(dataRow["badge_slot"]);

            if (!this.BadgeList.ContainsKey(Code))
            {
                this.BadgeList.Add(Code, new Badge(Code, Slot));
            }
        }
    }

    public int Count => this.BadgeList.Count;

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

    public bool HasBadgeSlot(string Badge)
    {
        if (this.BadgeList.ContainsKey(Badge))
        {
            return this.BadgeList[Badge].Slot > 0;
        }
        else
        {
            return false;
        }
    }

    public ICollection<Badge> GetBadges() => this.BadgeList.Values;

    public Badge GetBadge(string Badge)
    {
        if (this.BadgeList.ContainsKey(Badge))
        {
            return this.BadgeList[Badge];
        }
        else
        {
            return null;
        }
    }

    public bool HasBadge(string Badge)
    {
        if (string.IsNullOrEmpty(Badge))
        {
            return true;
        }

        return this.BadgeList.ContainsKey(Badge);
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

    public void RemoveBadge(string Badge)
    {
        if (!this.HasBadge(Badge))
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserBadgeDao.Delete(dbClient, this._userInstance.Id, Badge);
        }

        _ = this.BadgeList.Remove(this.GetBadge(Badge).Code);
    }

    public void Dispose()
    {
        this.BadgeList.Clear();
        GC.SuppressFinalize(this);
    }
}
