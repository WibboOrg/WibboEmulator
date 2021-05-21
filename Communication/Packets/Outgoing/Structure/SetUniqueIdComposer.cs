namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class SetUniqueIdComposer : ServerPacket
    {
        public SetUniqueIdComposer(string Id)
            : base(ServerPacketHeader.SECURITY_MACHINE)
        {
            this.WriteString(Id);
        }
    }
}
