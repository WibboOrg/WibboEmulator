namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

internal class RemoveGroupMemberEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var userId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (userId == session.GetUser().Id)
        {
            if (group.IsMember(userId))
            {
                group.DeleteMember(userId);
            }

            _ = session.GetUser().MyGroups.Remove(group.Id);

            if (group.IsAdmin(userId))
            {
                if (group.IsAdmin(userId))
                {
                    group.TakeAdmin(userId);
                }

                if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out var room))
                {
                    return;
                }

                var userRom = room.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
                if (userRom != null)
                {
                    userRom.RemoveStatus("flatctrl");
                    userRom.UpdateNeeded = true;

                    if (userRom.Client != null)
                    {
                        userRom.Client.SendPacket(new YouAreControllerComposer(0));
                    }
                }
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildMembershipDao.Delete(dbClient, groupId, userId);
            }

            session.SendPacket(new GroupInfoComposer(group, session));
            if (session.GetUser().FavouriteGroupId == groupId)
            {
                session.GetUser().FavouriteGroupId = 0;
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserStatsDao.UpdateRemoveGroupId(dbClient, userId);
                }

                if (group.AdminOnlyDeco == 0)
                {
                    if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out var room))
                    {
                        return;
                    }

                    var userRoom = room.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
                    if (userRoom != null)
                    {
                        userRoom.RemoveStatus("flatctrl");
                        userRoom.UpdateNeeded = true;

                        if (userRoom.Client != null)
                        {
                            userRoom.Client.SendPacket(new YouAreControllerComposer(0));
                        }
                    }
                }

                if (session.GetUser().InRoom && session.GetUser().CurrentRoom != null)
                {
                    var userRoom = session.GetUser().CurrentRoom.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
                    if (userRoom != null)
                    {
                        session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(group, userRoom.VirtualId));
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
            if (group.CreatorId == session.GetUser().Id || group.IsAdmin(session.GetUser().Id))
            {
                if (!group.IsMember(userId))
                {
                    return;
                }

                if (group.IsAdmin(userId) && group.CreatorId != session.GetUser().Id)
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupremoveuser.error", session.Langue));
                    return;
                }

                if (group.IsAdmin(userId))
                {
                    group.TakeAdmin(userId);
                }

                if (group.IsMember(userId))
                {
                    group.DeleteMember(userId);
                }

                var user = WibboEnvironment.GetUserById(userId);
                if (user != null)
                {
                    _ = user.MyGroups.Remove(group.Id);
                }

                var startIndex = ((1 - 1) * 14) + 14;

                var members = new List<User>();
                var memberIds = group.GetMembers.Skip(startIndex).Take(14).ToList();
                foreach (var id in memberIds.ToList())
                {
                    var groupMember = WibboEnvironment.GetUserById(id);
                    if (groupMember == null)
                    {
                        continue;
                    }

                    if (!members.Contains(groupMember))
                    {
                        members.Add(groupMember);
                    }
                }

                var finishIndex = 14 < members.Count ? 14 : members.Count;
                var membersCount = group.GetMembers.Count;

                session.SendPacket(new GroupMembersComposer(group, members.Take(finishIndex).ToList(), membersCount, 1, group.CreatorId == session.GetUser().Id || group.IsAdmin(session.GetUser().Id), 0, ""));
            }
        }
    }
}
