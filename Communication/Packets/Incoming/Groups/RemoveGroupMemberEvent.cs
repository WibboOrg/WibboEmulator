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

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var userId = packet.PopInt();

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (userId == Session.User.Id)
        {
            if (group.IsMember(userId))
            {
                group.DeleteMember(userId);
            }

            _ = Session.User.MyGroups.Remove(group.Id);

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

                var userRom = roomGroup.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
                if (userRom != null)
                {
                    userRom.RemoveStatus("flatctrl");
                    userRom.UpdateNeeded = true;

                    userRom.Client?.SendPacket(new YouAreControllerComposer(0));
                }
            }

            using var dbClient = DatabaseManager.Connection;

            GuildMembershipDao.Delete(dbClient, groupId, userId);

            Session.SendPacket(new GroupInfoComposer(group, Session));
            if (Session.User.FavouriteGroupId == groupId)
            {
                Session.User.FavouriteGroupId = 0;
                UserStatsDao.UpdateRemoveGroupId(dbClient, userId);

                if (!group.AdminOnlyDeco)
                {
                    if (!RoomManager.TryGetRoom(group.RoomId, out var roomGroup))
                    {
                        return;
                    }

                    var userRoom = roomGroup.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
                    if (userRoom != null)
                    {
                        userRoom.RemoveStatus("flatctrl");
                        userRoom.UpdateNeeded = true;

                        userRoom.Client?.SendPacket(new YouAreControllerComposer(0));
                    }
                }

                var room = Session.User.Room;
                if (room != null)
                {
                    var userRoom = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
                    if (userRoom != null)
                    {
                        room.SendPacket(new UpdateFavouriteGroupComposer(group, userRoom.VirtualId));
                    }

                    room.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
                }
                else
                {
                    Session.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
                }
            }
            return;
        }
        else
        {
            if (group.CreatorId == Session.User.Id || group.IsAdmin(Session.User.Id))
            {
                if (!group.IsMember(userId))
                {
                    return;
                }

                if (group.IsAdmin(userId) && group.CreatorId != Session.User.Id)
                {
                    Session.SendNotification(LanguageManager.TryGetValue("notif.groupremoveuser.error", Session.Language));
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

                var user = UserManager.GetUserById(userId);
                if (user != null)
                {
                    _ = user.MyGroups.Remove(group.Id);
                }

                var startIndex = ((1 - 1) * 14) + 14;

                var members = new List<User>();
                var memberIds = group.GetMembers.Skip(startIndex).Take(14).ToList();
                foreach (var id in memberIds.ToList())
                {
                    var groupMember = UserManager.GetUserById(id);
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

                Session.SendPacket(new GroupMembersComposer(group, members.Take(finishIndex).ToList(), membersCount, 1, group.CreatorId == Session.User.Id || group.IsAdmin(Session.User.Id), 0, ""));
            }
        }
    }
}
