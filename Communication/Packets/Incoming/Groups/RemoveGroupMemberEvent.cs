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
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveGroupMemberEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (UserId == Session.GetHabbo().Id)
            {
                if (Group.IsMember(UserId))
                {
                    Group.DeleteMember(UserId);
                }

                Session.GetHabbo().MyGroups.Remove(Group.Id);

                if (Group.IsAdmin(UserId))
                {
                    if (Group.IsAdmin(UserId))
                    {
                        Group.TakeAdmin(UserId);
                    }


                    if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
                    {
                        return;
                    }

                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                    if (User != null)
                    {
                        User.RemoveStatus("flatctrl 1");
                        User.UpdateNeeded = true;

                        if (User.GetClient() != null)
                        {
                            User.GetClient().SendPacket(new YouAreControllerComposer(0));
                        }
                    }
                }

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `group_memberships` WHERE `group_id` = @GroupId AND `user_id` = @UserId");
                    dbClient.AddParameter("GroupId", GroupId);
                    dbClient.AddParameter("UserId", UserId);
                    dbClient.RunQuery();
                }

                Session.SendPacket(new GroupInfoComposer(Group, Session));
                if (Session.GetHabbo().FavouriteGroupId == GroupId)
                {
                    Session.GetHabbo().FavouriteGroupId = 0;
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `user_stats` SET `group_id` = '0' WHERE `id` = @userId LIMIT 1");
                        dbClient.AddParameter("userId", UserId);
                        dbClient.RunQuery();
                    }

                    if (Group.AdminOnlyDeco == 0)
                    {
                        if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
                        {
                            return;
                        }

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                        if (User != null)
                        {
                            User.RemoveStatus("flatctrl 1");
                            User.UpdateNeeded = true;

                            if (User.GetClient() != null)
                            {
                                User.GetClient().SendPacket(new YouAreControllerComposer(0));
                            }
                        }
                    }

                    if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoom != null)
                    {
                        RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                        if (User != null)
                        {
                            Session.GetHabbo().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                        }

                        Session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                    }
                    else
                    {
                        Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                    }
                }
                return;
            }
            else
            {
                if (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id))
                {
                    if (!Group.IsMember(UserId))
                    {
                        return;
                    }

                    if (Group.IsAdmin(UserId) && Group.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupremoveuser.error", Session.Langue));
                        return;
                    }

                    if (Group.IsAdmin(UserId))
                    {
                        Group.TakeAdmin(UserId);
                    }

                    if (Group.IsMember(UserId))
                    {
                        Group.DeleteMember(UserId);
                    }

                    Habbo Habbo = ButterflyEnvironment.GetHabboById(UserId);
                    Habbo.MyGroups.Remove(Group.Id);

                    int StartIndex = (1 - 1) * 14 + 14;

                    List<Habbo> Members = new List<Habbo>();
                    List<int> MemberIds = Group.GetMembers.Skip(StartIndex).Take(14).ToList();
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

                    int FinishIndex = 14 < Members.Count ? 14 : Members.Count;
                    int MembersCount = Group.GetMembers.Count;

                    Session.SendPacket(new GroupMembersComposer(Group, Members.Take(FinishIndex).ToList(), MembersCount, 1, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), 0, ""));
                }
            }
        }
    }
}