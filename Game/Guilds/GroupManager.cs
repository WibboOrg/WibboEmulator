using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Users;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Guilds
{
    public class GroupManager
    {
        private readonly ConcurrentDictionary<int, Group> _groups;

        private readonly List<GroupBadgePart> _bases;
        private readonly List<GroupBadgePart> _symbols;
        private readonly List<GroupColour> _baseColours;
        private readonly Dictionary<int, GroupColour> _symbolColours;
        private readonly Dictionary<int, GroupColour> _backgroundColours;

        public GroupManager()
        {
            this._groups = new ConcurrentDictionary<int, Group>();

            this._bases = new List<GroupBadgePart>();
            this._symbols = new List<GroupBadgePart>();
            this._baseColours = new List<GroupColour>();
            this._symbolColours = new Dictionary<int, GroupColour>();
            this._backgroundColours = new Dictionary<int, GroupColour>();
        }

        public void Init()
        {
            this._bases.Clear();
            this._symbols.Clear();
            this._baseColours.Clear();
            this._symbolColours.Clear();
            this._backgroundColours.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable dItems = GuildItemDao.GetAll(dbClient);

                foreach (DataRow dRow in dItems.Rows)
                {
                    switch (dRow["type"].ToString())
                    {
                        case "base":
                            this._bases.Add(new GroupBadgePart(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                            break;

                        case "symbol":
                            this._symbols.Add(new GroupBadgePart(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                            break;

                        case "color":
                            this._baseColours.Add(new GroupColour(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;

                        case "color2":
                            this._symbolColours.Add(Convert.ToInt32(dRow["id"]), new GroupColour(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;

                        case "color3":
                            this._backgroundColours.Add(Convert.ToInt32(dRow["id"]), new GroupColour(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;
                    }
                }
            }
        }

        public bool TryGetGroup(int id, out Group Group)
        {
            Group = null;

            if (this._groups.ContainsKey(id))
            {
                return this._groups.TryGetValue(id, out Group);
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataRow Row = GuildDao.GetOne(dbClient, id);

                if (Row == null)
                {
                    return false;
                }

                Group = new Group(
                        Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["desc"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["owner_id"]),
                        Convert.ToInt32(Row["created"]), Convert.ToInt32(Row["state"]), Convert.ToInt32(Row["colour1"]), Convert.ToInt32(Row["colour2"]), Convert.ToInt32(Row["admindeco"]), Convert.ToInt32(Row["has_forum"]) == 1);

                this._groups.TryAdd(Group.Id, Group);
            }

            return true;
        }

        public bool TryCreateGroup(User Player, string Name, string Description, int RoomId, string Badge, int Colour1, int Colour2, out Group Group)
        {
            Group = new Group(0, Name, Description, Badge, RoomId, Player.Id, ButterflyEnvironment.GetUnixTimestamp(), 0, Colour1, Colour2, 0, false);
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Badge))
            {
                return false;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Group.Id = GuildDao.Insert(dbClient, Group.Name, Group.Description, Group.CreatorId, Group.Badge, Group.RoomId, Group.Colour1, Group.Colour2);

                Group.AddMember(Player.Id);
                Group.MakeAdmin(Player.Id);

                Player.MyGroups.Add(Group.Id);

                if (!this._groups.TryAdd(Group.Id, Group))
                {
                    return false;
                }
                else
                {
                    RoomDao.UpdateGroupId(dbClient, Group.Id, Group.RoomId);
                    RoomRightDao.Delete(dbClient, RoomId);
                }
            }
            return true;
        }

        public string GetColourCode(int id, bool colourOne)
        {
            if (colourOne)
            {
                if (this._symbolColours.ContainsKey(id))
                {
                    return this._symbolColours[id].Colour;
                }
            }
            else
            {
                if (this._backgroundColours.ContainsKey(id))
                {
                    return this._backgroundColours[id].Colour;
                }
            }

            return "";
        }

        public void DeleteGroup(int id)
        {
            Group Group = null;
            if (this._groups.ContainsKey(id))
            {
                this._groups.TryRemove(id, out Group);
            }

            if (Group != null)
            {
                Group.Dispose();
            }
        }

        public List<Group> GetGroupsForUser(List<int> GroupIds)
        {
            List<Group> Groups = new List<Group>();

            foreach (int Id in GroupIds)
            {
                if (this.TryGetGroup(Id, out Group Group))
                {
                    Groups.Add(Group);
                }
            }
            return Groups;
        }


        public ICollection<GroupBadgePart> BadgeBases => this._bases;

        public ICollection<GroupBadgePart> BadgeSymbols => this._symbols;

        public ICollection<GroupColour> BadgeBaseColours => this._baseColours;

        public ICollection<GroupColour> BadgeSymbolColours => this._symbolColours.Values;

        public ICollection<GroupColour> BadgeBackColours => this._backgroundColours.Values;
    }
}
