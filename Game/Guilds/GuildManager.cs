using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Users;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Guilds
{
    public class GuildManager
    {
        private readonly ConcurrentDictionary<int, Guild> _groups;

        private readonly List<GuildBadgePart> _bases;
        private readonly List<GuildBadgePart> _symbols;
        private readonly List<GuildColour> _baseColours;
        private readonly Dictionary<int, GuildColour> _symbolColours;
        private readonly Dictionary<int, GuildColour> _backgroundColours;

        public GuildManager()
        {
            this._groups = new ConcurrentDictionary<int, Guild>();

            this._bases = new List<GuildBadgePart>();
            this._symbols = new List<GuildBadgePart>();
            this._baseColours = new List<GuildColour>();
            this._symbolColours = new Dictionary<int, GuildColour>();
            this._backgroundColours = new Dictionary<int, GuildColour>();
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
                            this._bases.Add(new GuildBadgePart(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                            break;

                        case "symbol":
                            this._symbols.Add(new GuildBadgePart(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                            break;

                        case "color":
                            this._baseColours.Add(new GuildColour(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;

                        case "color2":
                            this._symbolColours.Add(Convert.ToInt32(dRow["id"]), new GuildColour(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;

                        case "color3":
                            this._backgroundColours.Add(Convert.ToInt32(dRow["id"]), new GuildColour(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;
                    }
                }
            }
        }

        public bool TryGetGroup(int id, out Guild Group)
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

                Group = new Guild(
                        Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["desc"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["owner_id"]),
                        Convert.ToInt32(Row["created"]), Convert.ToInt32(Row["state"]), Convert.ToInt32(Row["colour1"]), Convert.ToInt32(Row["colour2"]), Convert.ToInt32(Row["admindeco"]), Convert.ToInt32(Row["has_forum"]) == 1);

                this._groups.TryAdd(Group.Id, Group);
            }

            return true;
        }

        public bool TryCreateGroup(User Player, string Name, string Description, int RoomId, string Badge, int Colour1, int Colour2, out Guild Group)
        {
            Group = new Guild(0, Name, Description, Badge, RoomId, Player.Id, ButterflyEnvironment.GetUnixTimestamp(), 0, Colour1, Colour2, 0, false);
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
            Guild Group = null;
            if (this._groups.ContainsKey(id))
            {
                this._groups.TryRemove(id, out Group);
            }

            if (Group != null)
            {
                Group.Dispose();
            }
        }

        public List<Guild> GetGroupsForUser(List<int> GroupIds)
        {
            List<Guild> Groups = new List<Guild>();

            foreach (int Id in GroupIds)
            {
                if (this.TryGetGroup(Id, out Guild Group))
                {
                    Groups.Add(Group);
                }
            }
            return Groups;
        }


        public ICollection<GuildBadgePart> BadgeBases => this._bases;

        public ICollection<GuildBadgePart> BadgeSymbols => this._symbols;

        public ICollection<GuildColour> BadgeBaseColours => this._baseColours;

        public ICollection<GuildColour> BadgeSymbolColours => this._symbolColours.Values;

        public ICollection<GuildColour> BadgeBackColours => this._backgroundColours.Values;
    }
}
