namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class NewGroupInfoComposer : ServerPacket
    {
        public NewGroupInfoComposer(int RoomId, int GroupId)
            : base(ServerPacketHeader.GROUP_PURCHASED)
        {
            this.WriteInteger(RoomId);
            this.WriteInteger(GroupId);
        }
    }
}
