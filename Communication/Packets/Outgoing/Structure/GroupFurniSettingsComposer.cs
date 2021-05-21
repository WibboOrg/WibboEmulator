using Butterfly.HabboHotel.Groups;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class GroupFurniSettingsComposer : ServerPacket
    {
        public GroupFurniSettingsComposer(Group Group, int ItemId, int UserId)
            : base(ServerPacketHeader.GroupFurniSettingsMessageComposer)
        {
            this.WriteInteger(ItemId);//Item Id
            this.WriteInteger(Group.Id);//Group Id?
            this.WriteString(Group.Name);
            this.WriteInteger(Group.RoomId);//RoomId
            this.WriteBoolean(Group.IsMember(UserId));//Member?
            this.WriteBoolean(Group.ForumEnabled);//Has a forum
        }
    }
}