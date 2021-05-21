namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class UserRemoveComposer : ServerPacket
    {
        public UserRemoveComposer(int Id)
            : base(ServerPacketHeader.UNIT_REMOVE)
        {
            this.WriteString(Id.ToString());
        }
    }
}
