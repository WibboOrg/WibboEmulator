using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Users.Badges
{
    public class BadgeComponent : IDisposable
    {
        private readonly User _userInstance;
        private readonly Dictionary<string, Badge> _badges;

        public BadgeComponent(User user)
        {
            this._userInstance = user;
            this._badges = new Dictionary<string, Badge>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            DataTable table = UserBadgeDao.GetAll(dbClient, this._userInstance.Id);

            foreach (DataRow dataRow in table.Rows)
            {
                string Code = (string)dataRow["badge_id"];
                int Slot = Convert.ToInt32(dataRow["badge_slot"]);

                if (!this._badges.ContainsKey(Code))
                    this._badges.Add(Code, new Badge(Code, Slot));
            }
        }

        public int Count => this._badges.Count;

        public int EquippedCount
        {
            get
            {
                int num = 0;
                foreach (Badge badge in (IEnumerable)this._badges.Values)
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

        public Dictionary<string, Badge> BadgeList => this._badges;

        public bool HasBadgeSlot(string Badge)
        {
            if (this._badges.ContainsKey(Badge))
            {
                return this._badges[Badge].Slot > 0;
            }
            else
            {
                return false;
            }
        }

        public ICollection<Badge> GetBadges()
        {
            return this._badges.Values;
        }

        public Badge GetBadge(string Badge)
        {
            if (this._badges.ContainsKey(Badge))
            {
                return this._badges[Badge];
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

            return this._badges.ContainsKey(Badge);
        }

        public void GiveBadge(string Badge, bool InDatabase)
        {
            this.GiveBadge(Badge, 0, InDatabase);
        }

        public void GiveBadge(string Badge, int Slot, bool InDatabase)
        {
            if (this.HasBadge(Badge))
            {
                return;
            }

            if (InDatabase)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserBadgeDao.Insert(dbClient, this._userInstance.Id, Slot, Badge);
                }
            }
            this._badges.Add(Badge, new Badge(Badge, Slot));
        }

        public void ResetSlots()
        {
            foreach (Badge badge in (IEnumerable)this._badges.Values)
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                UserBadgeDao.Delete(dbClient, this._userInstance.Id, Badge);

            this._badges.Remove(this.GetBadge(Badge).Code);
        }

        public void Dispose()
        {
            this._badges.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
