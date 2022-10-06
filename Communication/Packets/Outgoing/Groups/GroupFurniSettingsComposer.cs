namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Groups;

internal class GroupFurniSettingsComposer : ServerPacket
{
    public GroupFurniSettingsComposer(Group group, int itemId, int userId)
        : base(ServerPacketHeader.FURNITURE_GROUP_CONTEXT_MENU_INFO)
    {
        this.WriteInteger(itemId);//Item Id
        this.WriteInteger(group.Id);//Group Id?
        this.WriteString(group.Name);
        this.WriteInteger(group.RoomId);//RoomId
        this.WriteBoolean(group.IsMember(userId));//Member?
        this.WriteBoolean(group.ForumEnabled);//Has a forum
    }
}
