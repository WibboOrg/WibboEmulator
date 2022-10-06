namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Daos.Guild;

internal class RemoveGroupMemberEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var GroupId = packet.PopInt();
        var UserId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        if (UserId == session.GetUser().Id)
        {
            if (Group.IsMember(UserId))
            {
                Group.DeleteMember(UserId);
            }

            _ = session.GetUser().MyGroups.Remove(Group.Id);

            if (Group.IsAdmin(UserId))
            {
                if (Group.IsAdmin(UserId))
                {
                    Group.TakeAdmin(UserId);
                }

                if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out var Room))
                {
                    return;
                }

                var User = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
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

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildMembershipDao.Delete(dbClient, GroupId, UserId);
            }

            session.SendPacket(new GroupInfoComposer(Group, session));
            if (session.GetUser().FavouriteGroupId == GroupId)
            {
                session.GetUser().FavouriteGroupId = 0;
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserStatsDao.UpdateRemoveGroupId(dbClient, UserId);
                }

                if (Group.AdminOnlyDeco == 0)
                {
                    if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out var Room))
                    {
                        return;
                    }

                    var User = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
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

                if (session.GetUser().InRoom && session.GetUser().CurrentRoom != null)
                {
                    var User = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
                    if (User != null)
                    {
                        session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                    }

                    session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
                }
                else
                {
                    session.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
                }
            }
            return;
        }
        else
        {
            if (Group.CreatorId == session.GetUser().Id || Group.IsAdmin(session.GetUser().Id))
            {
                if (!Group.IsMember(UserId))
                {
                    return;
                }

                if (Group.IsAdmin(UserId) && Group.CreatorId != session.GetUser().Id)
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupremoveuser.error", session.Langue));
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

                var user = WibboEnvironment.GetUserById(UserId);
                if (user != null)
                {
                    _ = user.MyGroups.Remove(Group.Id);
                }

                var StartIndex = ((1 - 1) * 14) + 14;

                var Members = new List<User>();
                var MemberIds = Group.GetMembers.Skip(StartIndex).Take(14).ToList();
                foreach (var Id in MemberIds.ToList())
                {
                    var GroupMember = WibboEnvironment.GetUserById(Id);
                    if (GroupMember == null)
                    {
                        continue;
                    }

                    if (!Members.Contains(GroupMember))
                    {
                        Members.Add(GroupMember);
                    }
                }

                var FinishIndex = 14 < Members.Count ? 14 : Members.Count;
                var MembersCount = Group.GetMembers.Count;

                session.SendPacket(new GroupMembersComposer(Group, Members.Take(FinishIndex).ToList(), MembersCount, 1, Group.CreatorId == session.GetUser().Id || Group.IsAdmin(session.GetUser().Id), 0, ""));
            }
        }
    }
}
