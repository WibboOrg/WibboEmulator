using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;

using System.Collections;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Users.Badges
{
    public class BadgeComponent
    {
        private readonly Dictionary<string, Badge> _badges;
        private readonly int _userId;

        public BadgeComponent(int userId, List<Badge> data)
        {
            this._badges = new Dictionary<string, Badge>();
            foreach (Badge badge in data)
            {
                if (!this._badges.ContainsKey(badge.Code))
                {
                    this._badges.Add(badge.Code, badge);
                }
            }

            this._userId = userId;
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

        public void Destroy()
        {
            this._badges.Clear();
        }

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
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + this._userId + ",@badge," + Slot + ")");
                    queryreactor.AddParameter("badge", Badge);
                    queryreactor.RunQuery();
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

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + this._userId + " LIMIT 1");
                queryreactor.AddParameter("badge", Badge);
                queryreactor.RunQuery();
            }
            this._badges.Remove(this.GetBadge(Badge).Code);
        }

        public ServerPacket Serialize()
        {
            List<Badge> list = new List<Badge>();
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.USER_BADGES);
            serverMessage.WriteInteger(this.Count);
            foreach (Badge badge in (IEnumerable)this._badges.Values)
            {
                serverMessage.WriteInteger(0);
                serverMessage.WriteString(badge.Code);
                if (badge.Slot > 0)
                {
                    list.Add(badge);
                }
            }
            serverMessage.WriteInteger(list.Count);
            foreach (Badge badge in list)
            {
                serverMessage.WriteInteger(badge.Slot);
                serverMessage.WriteString(badge.Code);
            }
            return serverMessage;
        }
    }
}
