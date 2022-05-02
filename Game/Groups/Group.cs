using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Groups
{
    public class Group
    {
        public int Id;
        public string Name;
        public int AdminOnlyDeco;
        public string Badge;
        public int CreateTime;
        public int CreatorId;
        public string Description;
        public int RoomId;
        public int Colour1;
        public int Colour2;
        public bool ForumEnabled;
        public GroupType GroupType;
        public bool HasForum;
        private readonly List<int> _members;
        private readonly List<int> _requests;
        private readonly List<int> _administrators;

        public Group(int Id, string Name, string Description, string Badge, int RoomId, int Owner, int Time, int Type, int Colour1, int Colour2, int AdminOnlyDeco, bool HasForum)
        {
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.RoomId = RoomId;
            this.Badge = Badge;
            this.CreateTime = Time;
            this.CreatorId = Owner;
            this.Colour1 = (Colour1 == 0) ? 1 : Colour1;
            this.Colour2 = (Colour2 == 0) ? 1 : Colour2;
            this.HasForum = HasForum;

            switch (Type)
            {
                case 0:
                    this.GroupType = GroupType.OPEN;
                    break;
                case 1:
                    this.GroupType = GroupType.LOCKED;
                    break;
                case 2:
                    this.GroupType = GroupType.PRIVATE;
                    break;
            }

            this.AdminOnlyDeco = AdminOnlyDeco;
            this.ForumEnabled = false;

            this._members = new List<int>();
            this._requests = new List<int>();
            this._administrators = new List<int>();

            this.InitMembers();
        }

        public void InitMembers()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetMembers = GuildMembershipDao.GetAll(dbClient, this.Id);

                if (GetMembers != null)
                {
                    foreach (DataRow Row in GetMembers.Rows)
                    {
                        int UserId = Convert.ToInt32(Row["user_id"]);
                        bool IsAdmin = Convert.ToInt32(Row["rank"]) != 0;

                        if (IsAdmin)
                        {
                            if (!this._administrators.Contains(UserId))
                            {
                                this._administrators.Add(UserId);
                            }
                        }
                        else
                        {
                            if (!this._members.Contains(UserId))
                            {
                                this._members.Add(UserId);
                            }
                        }
                    }
                }

                DataTable GetRequests = GuildRequestDao.GetAll(dbClient, this.Id);

                if (GetRequests != null)
                {
                    foreach (DataRow Row in GetRequests.Rows)
                    {
                        int UserId = Convert.ToInt32(Row["user_id"]);

                        if (this._members.Contains(UserId) || this._administrators.Contains(UserId))
                        {
                            GuildRequestDao.Delete(dbClient, this.Id, UserId);
                        }
                        else if (!this._requests.Contains(UserId))
                        {
                            this._requests.Add(UserId);
                        }
                    }
                }
            }
        }

        public List<int> GetMembers => this._members.ToList();

        public List<int> GetRequests => this._requests.ToList();

        public List<int> GetAdministrators => this._administrators.ToList();

        public List<int> GetAllMembers
        {
            get
            {
                List<int> Members = new List<int>(this._administrators.ToList());
                Members.AddRange(this._members.ToList());

                return Members;
            }
        }

        public int MemberCount => this._members.Count + this._administrators.Count;

        public int RequestCount => this._requests.Count;

        public bool IsMember(int Id)
        {
            return this._members.Contains(Id) || this._administrators.Contains(Id);
        }

        public bool IsAdmin(int Id)
        {
            return this._administrators.Contains(Id);
        }

        public bool HasRequest(int Id)
        {
            return this._requests.Contains(Id);
        }

        public void MakeAdmin(int userId)
        {
            if (this._members.Contains(userId))
            {
                this._members.Remove(userId);
            }

            if (this._administrators.Contains(userId))
            {
                return;
            }

            this._administrators.Add(userId);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildMembershipDao.UpdateRank(dbClient, this.Id, userId, 1);
            }
        }

        public void TakeAdmin(int userId)
        {
            if (!this._administrators.Contains(userId))
            {
                return;
            }

            this._administrators.Remove(userId);

            if (!this._members.Contains(userId))
            {
                this._members.Add(userId);
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildMembershipDao.UpdateRank(dbClient, this.Id, userId, 0);
            }
        }

        public void AddMember(int userId)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (this.IsAdmin(userId))
                {
                    GuildMembershipDao.UpdateRank(dbClient, this.Id, userId, 0);
                    this._administrators.Remove(userId);
                    this._members.Add(userId);
                }
                else if (this.GroupType == GroupType.LOCKED)
                {
                    GuildRequestDao.Insert(dbClient, this.Id, userId);
                    if (!this._requests.Contains(userId))
                    {
                        this._requests.Add(userId);
                    }
                }
                else
                {
                    GuildMembershipDao.Insert(dbClient, this.Id, userId);
                    if (!this._members.Contains(userId))
                    {
                        this._members.Add(userId);
                    }
                }
            }
        }

        public void DeleteMember(int userId)
        {
            if (this.IsMember(userId))
            {
                if (this._members.Contains(userId))
                {
                    this._members.Remove(userId);
                }
            }
            else if (this.IsAdmin(userId))
            {
                if (this._administrators.Contains(userId))
                {
                    this._administrators.Remove(userId);
                }
            }
            else
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildMembershipDao.Delete(dbClient, this.Id, userId);
            }
        }

        public void HandleRequest(int userId, bool Accepted)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (Accepted)
                {
                    GuildMembershipDao.Insert(dbClient, this.Id, userId);

                    if (!this._members.Contains(userId))
                    {
                        this._members.Add(userId);
                    }
                }

                GuildRequestDao.Delete(dbClient, this.Id, userId);
            }

            if (this._requests.Contains(userId))
            {
                this._requests.Remove(userId);
            }
        }

        public void ClearRequests()
        {
            this._requests.Clear();
        }

        public void Dispose()
        {
            this._requests.Clear();
            this._members.Clear();
            this._administrators.Clear();
        }
    }
}
