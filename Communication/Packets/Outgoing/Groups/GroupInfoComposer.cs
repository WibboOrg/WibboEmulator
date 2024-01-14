namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

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
        this.WriteString((WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(group.RoomId) == null) ? "No room found.." : WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(group.RoomId).Name);    // room name
        this.WriteInteger(group.CreatorId == session.User.Id ? 3 : group.HasRequest(session.User.Id) ? 2 : group.IsMember(session.User.Id) ? 1 : 0);
        this.WriteInteger(group.MemberCount); // Members
        this.WriteBoolean(false);//?? CHANGED
        this.WriteString(origin.Day + "-" + origin.Month + "-" + origin.Year);
        this.WriteBoolean(group.CreatorId == session.User.Id);
        this.WriteBoolean(group.IsAdmin(session.User.Id)); // admin
        this.WriteString(userCreator != null ? userCreator.Username : "");
        this.WriteBoolean(newWindow); // Show group info
        this.WriteBoolean(group.AdminOnlyDeco == false); // Any user can place furni in home room
        this.WriteInteger(group.CreatorId == session.User.Id ? group.RequestCount : group.IsAdmin(session.User.Id) ? group.RequestCount : group.IsMember(session.User.Id) ? 0 : 0); // Pending users
        //base.WriteInteger(0);//what the fuck
        this.WriteBoolean(group == null || group.ForumEnabled);
    }
}
