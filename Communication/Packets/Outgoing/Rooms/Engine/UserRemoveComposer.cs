namespace Wibbo.Communication.Packets.Outgoing.Rooms.Engine
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
