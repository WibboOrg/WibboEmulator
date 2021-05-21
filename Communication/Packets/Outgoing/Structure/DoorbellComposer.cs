namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class DoorbellComposer : ServerPacket
    {
        public DoorbellComposer(string Username)
            : base(ServerPacketHeader.ROOM_DOORBELL_ADD)
        {
            this.WriteString(Username);
        }
    }
}
