namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class GroupInfoComposer : ServerPacket
{
    public GroupInfoComposer(Group Group, GameClient session, bool NewWindow = false)
        : base(ServerPacketHeader.GROUP_INFO)
    {
        var Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Group.CreateTime);

        this.WriteInteger(Group.Id);
        this.WriteBoolean(true);
        this.WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
        this.WriteString(Group.Name);
        this.WriteString(Group.Description);
        this.WriteString(Group.Badge);
        this.WriteInteger(Group.RoomId);
        this.WriteString((WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId) == null) ? "No room found.." : WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId).Name);    // room name
        this.WriteInteger(Group.CreatorId == session.GetUser().Id ? 3 : Group.HasRequest(session.GetUser().Id) ? 2 : Group.IsMember(session.GetUser().Id) ? 1 : 0);
        this.WriteInteger(Group.MemberCount); // Members
        this.WriteBoolean(false);//?? CHANGED
        this.WriteString(Origin.Day + "-" + Origin.Month + "-" + Origin.Year);
        this.WriteBoolean(Group.CreatorId == session.GetUser().Id);
        this.WriteBoolean(Group.IsAdmin(session.GetUser().Id)); // admin
        this.WriteString(WibboEnvironment.GetUsernameById(Group.CreatorId));
        this.WriteBoolean(NewWindow); // Show group info
        this.WriteBoolean(Group.AdminOnlyDeco == 0); // Any user can place furni in home room
        this.WriteInteger(Group.CreatorId == session.GetUser().Id ? Group.RequestCount : Group.IsAdmin(session.GetUser().Id) ? Group.RequestCount : Group.IsMember(session.GetUser().Id) ? 0 : 0); // Pending users
        //base.WriteInteger(0);//what the fuck
        this.WriteBoolean(Group != null ? Group.ForumEnabled : true);
    }
}
