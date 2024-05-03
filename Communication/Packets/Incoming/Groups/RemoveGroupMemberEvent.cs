namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users;

internal sealed class RemoveGroupMemberEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var userId = packet.PopInt();

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (userId == session.User.Id)
        {
            if (group.IsMember(userId))
            {
                group.DeleteMember(userId);
            }

            _ = session.User.MyGroups.Remove(group.Id);

            if (group.IsAdmin(userId))
            {
                if (group.IsAdmin(userId))
                {
                    group.TakeAdmin(userId);
                }

                if (!RoomManager.TryGetRoom(group.RoomId, out var roomGroup))
                {
                    return;
                }

                var userRom = roomGroup.RoomUserManager.GetRoomUserByUserId(session.User.Id);
                if (userRom != null)
                {
                    userRom.RemoveStatus("flatctrl");
                    userRom.UpdateNeeded = true;

                    userRom.Client?.SendPacket(new YouAreControllerComposer(0));
                }
            }

            using var dbClient = DatabaseManager.Connection;

            GuildMembershipDao.Delete(dbClient, groupId, userId);

            session.SendPacket(new GroupInfoComposer(group, session));
            if (session.User.FavouriteGroupId == groupId)
            {
                session.User.FavouriteGroupId = 0;
                UserStatsDao.UpdateRemoveGroupId(dbClient, userId);

                if (group.AdminOnlyDeco == false)
                {
                    if (!RoomManager.TryGetRoom(group.RoomId, out var roomGroup))
                    {
                        return;
                    }

                    var userRoom = roomGroup.RoomUserManager.GetRoomUserByUserId(session.User.Id);
                    if (userRoom != null)
                    {
                        userRoom.RemoveStatus("flatctrl");
                        userRoom.UpdateNeeded = true;

                        userRoom.Client?.SendPacket(new YouAreControllerComposer(0));
                    }
                }

                var room = session.User.Room;
                if (room != null)
                {
                    var userRoom = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
                    if (userRoom != null)
                    {
                        room.SendPacket(new UpdateFavouriteGroupComposer(group, userRoom.VirtualId));
                    }

                    room.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
                }
                else
                {
                    session.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
                }
            }
            return;
        }
        else
        {
            if (group.CreatorId == session.User.Id || group.IsAdmin(session.User.Id))
            {
                if (!group.IsMember(userId))
                {
                    return;
                }

                if (group.IsAdmin(userId) && group.CreatorId != session.User.Id)
                {
                    session.SendNotification(LanguageManager.TryGetValue("notif.groupremoveuser.error", session.Language));
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

                session.SendPacket(new GroupMembersComposer(group, members.Take(finishIndex).ToList(), membersCount, 1, group.CreatorId == session.User.Id || group.IsAdmin(session.User.Id), 0, ""));
            }
        }
    }
}
