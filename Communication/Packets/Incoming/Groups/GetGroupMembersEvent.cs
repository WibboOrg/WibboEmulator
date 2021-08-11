using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Communication.Packets.Outgoing.Inventory;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Navigator.New;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Communication.Packets.Outgoing.Rooms;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetGroupMembersEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            int GroupId = Packet.PopInt();
            int Page = Packet.PopInt();
            string SearchVal = Packet.PopString();
            int RequestType = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            int StartIndex = (Page - 1) * 14 + 14;
            List<Habbo> Members = new List<Habbo>();
            int MemberCount = 0;

            switch (RequestType)
            {
                case 0:
                    MemberCount = Group.GetAllMembers.Count();
                    List<int> MemberIds = null;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        MemberIds = this.GetSearchMembres(Group.Id, SearchVal);
                    }
                    else
                    {
                        MemberIds = Group.GetAllMembers.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int Id in MemberIds.ToList())
                    {
                        Habbo GroupMember = ButterflyEnvironment.GetHabboById(Id);
                        if (GroupMember == null)
                        {
                            continue;
                        }

                        if (!Members.Contains(GroupMember))
                        {
                            Members.Add(GroupMember);
                        }
                    }
                    break;
                case 1:
                    MemberCount = Group.GetAdministrators.Count();
                    List<int> AdminIds = null;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        AdminIds = this.GetSearchAdmins(Group.Id, SearchVal);
                    }
                    else
                    {
                        AdminIds = Group.GetAdministrators.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int User in AdminIds.ToList())
                    {
                        Habbo GroupMember = ButterflyEnvironment.GetHabboById(User);
                        if (GroupMember == null)
                        {
                            continue;
                        }

                        if (!Members.Contains(GroupMember))
                        {
                            Members.Add(GroupMember);
                        }
                    }
                    break;
                case 2:
                    MemberCount = Group.GetRequests.Count();
                    List<int> RequestIds = null;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        RequestIds = this.GetSearchRequests(Group.Id, SearchVal);
                    }
                    else
                    {
                        RequestIds = Group.GetRequests.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int Id in RequestIds.ToList())
                    {
                        Habbo GroupMember = ButterflyEnvironment.GetHabboById(Id);
                        if (GroupMember == null)
                        {
                            continue;
                        }

                        if (!Members.Contains(GroupMember))
                        {
                            Members.Add(GroupMember);
                        }
                    }
                    break;
            }

            //if (!string.IsNullOrEmpty(SearchVal))
            //{
            //Members = Members.Where(x => x.Username.StartsWith(SearchVal)).ToList();
            //}

            Session.SendPacket(new GroupMembersComposer(Group, Members.ToList(), MemberCount, Page, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), RequestType, SearchVal));

        }

        private List<int> GetSearchRequests(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT users.id FROM group_requests INNER JOIN users ON group_requests.user_id = users.id WHERE group_requests.group_id = @gid AND users.username LIKE @username LIMIT 14;");
                dbClient.AddParameter("gid", GroupeId);
                dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
                MembresTable = dbClient.GetTable();
            }

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                {
                    MembersId.Add(Convert.ToInt32(row["id"]));
                }
            }

            return MembersId;
        }

        private List<int> GetSearchAdmins(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT users.id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND group_memberships.rank > '0' AND users.username LIKE @username LIMIT 14;");
                dbClient.AddParameter("gid", GroupeId);
                dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
                MembresTable = dbClient.GetTable();
            }

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                {
                    MembersId.Add(Convert.ToInt32(row["id"]));
                }
            }

            return MembersId;
        }

        private List<int> GetSearchMembres(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT users.id AS id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND users.username LIKE @username LIMIT 14;");
                dbClient.AddParameter("gid", GroupeId);
                dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
                MembresTable = dbClient.GetTable();
            }

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                {
                    MembersId.Add(Convert.ToInt32(row["id"]));
                }
            }

            return MembersId;
        }
    }
}