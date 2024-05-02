namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal sealed class GroupInfoComposer : ServerPacket
{
    public GroupInfoComposer(Group group, GameClient session, bool newWindow = false)
        : base(ServerPacketHeader.GROUP_INFO)
    {
        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(group.CreateTime);

        var userCreator = WibboEnvironment.GetUserById(group.CreatorId);

        this.WriteInteger(group.Id);
        this.WriteBoolean(true);
        this.WriteInteger(group.GroupType == GroupType.Open ? 0 : group.GroupType == GroupType.Locked ? 1 : 2);
        this.WriteString(group.Name);
        this.WriteString(group.Description);
        this.WriteString(group.Badge);
        this.WriteInteger(group.RoomId);
        this.WriteString((RoomManager.GenerateRoomData(group.RoomId) == null) ? "Unknown" : RoomManager.GenerateRoomData(group.RoomId).Name);
        this.WriteInteger(group.CreatorId == session.User.Id ? 3 : group.HasRequest(session.User.Id) ? 2 : group.IsMember(session.User.Id) ? 1 : 0);
        this.WriteInteger(group.MemberCount);
        this.WriteBoolean(false);
        this.WriteString(origin.Day + "-" + origin.Month + "-" + origin.Year);
        this.WriteBoolean(group.CreatorId == session.User.Id);
        this.WriteBoolean(group.IsAdmin(session.User.Id));
        this.WriteString(userCreator != null ? userCreator.Username : "");
        this.WriteBoolean(newWindow);
        this.WriteBoolean(group.AdminOnlyDeco == false);
        this.WriteInteger(group.CreatorId == session.User.Id ? group.RequestCount : group.IsAdmin(session.User.Id) ? group.RequestCount : group.IsMember(session.User.Id) ? 0 : 0);
        this.WriteBoolean(group == null || group.ForumEnabled);
    }
}
