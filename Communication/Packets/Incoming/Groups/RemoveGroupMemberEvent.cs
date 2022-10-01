using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RemoveGroupMemberEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (UserId == Session.GetUser().Id)
            {
                if (Group.IsMember(UserId))
                {
                    Group.DeleteMember(UserId);
                }

                Session.GetUser().MyGroups.Remove(Group.Id);

                if (Group.IsAdmin(UserId))
                {
                    if (Group.IsAdmin(UserId))
                    {
                        Group.TakeAdmin(UserId);
                    }

                    if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
                    {
                        return;
                    }

                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                    if (User != null)
                    {
                        User.RemoveStatus("flatctrl");
                        User.UpdateNeeded = true;

                        if (User.GetClient() != null)
                        {
                            User.GetClient().SendPacket(new YouAreControllerComposer(0));
                        }
                    }
                }

                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    GuildMembershipDao.Delete(dbClient, GroupId, UserId);
                }

                Session.SendPacket(new GroupInfoComposer(Group, Session));
                if (Session.GetUser().FavouriteGroupId == GroupId)
                {
                    Session.GetUser().FavouriteGroupId = 0;
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserStatsDao.UpdateRemoveGroupId(dbClient, UserId);
                    }

                    if (Group.AdminOnlyDeco == 0)
                    {
                        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
                        {
                            return;
                        }

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                        if (User != null)
                        {
                            User.RemoveStatus("flatctrl");
                            User.UpdateNeeded = true;

                            if (User.GetClient() != null)
                            {
                                User.GetClient().SendPacket(new YouAreControllerComposer(0));
                            }
                        }
                    }

                    if (Session.GetUser().InRoom && Session.GetUser().CurrentRoom != null)
                    {
                        RoomUser User = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                        if (User != null)
                        {
                            Session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                        }

                        Session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
                    }
                    else
                    {
                        Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
                    }
                }
                return;
            }
            else
            {
                if (Group.CreatorId == Session.GetUser().Id || Group.IsAdmin(Session.GetUser().Id))
                {
                    if (!Group.IsMember(UserId))
                    {
                        return;
                    }

                    if (Group.IsAdmin(UserId) && Group.CreatorId != Session.GetUser().Id)
                    {
                        Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupremoveuser.error", Session.Langue));
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

                    User user = WibboEnvironment.GetUserById(UserId);
                    if(user != null)
                        user.MyGroups.Remove(Group.Id);

                    int StartIndex = (1 - 1) * 14 + 14;

                    List<User> Members = new List<User>();
                    List<int> MemberIds = Group.GetMembers.Skip(StartIndex).Take(14).ToList();
                    foreach (int Id in MemberIds.ToList())
                    {
                        User GroupMember = WibboEnvironment.GetUserById(Id);
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

                    Session.SendPacket(new GroupMembersComposer(Group, Members.Take(FinishIndex).ToList(), MembersCount, 1, (Group.CreatorId == Session.GetUser().Id || Group.IsAdmin(Session.GetUser().Id)), 0, ""));
                }
            }
        }
    }
}