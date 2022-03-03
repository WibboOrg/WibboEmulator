namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class UnknownGroupComposer : ServerPacket
    {
        public UnknownGroupComposer(int groupId, int userId)
            : base(ServerPacketHeader.GROUP_MEMBERS_REFRESH)
        {
            this.WriteInteger(groupId);
            this.WriteInteger(userId);
        }
    }
}
