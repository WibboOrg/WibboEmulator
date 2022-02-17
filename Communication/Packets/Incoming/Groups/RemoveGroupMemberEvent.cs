using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveGroupMemberEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
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
                    GuildMembershipDao.Delete(dbClient, GroupId, UserId);
                }

                Session.SendPacket(new GroupInfoComposer(Group, Session));
                if (Session.GetHabbo().FavouriteGroupId == GroupId)
                {
                    Session.GetHabbo().FavouriteGroupId = 0;
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserStatsDao.UpdateRemoveGroupId(dbClient, UserId);
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

                    User Habbo = ButterflyEnvironment.GetHabboById(UserId);
                    Habbo.MyGroups.Remove(Group.Id);

                    int StartIndex = (1 - 1) * 14 + 14;

                    List<User> Members = new List<User>();
                    List<int> MemberIds = Group.GetMembers.Skip(StartIndex).Take(14).ToList();
                    foreach (int Id in MemberIds.ToList())
                    {
                        User GroupMember = ButterflyEnvironment.GetHabboById(Id);
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