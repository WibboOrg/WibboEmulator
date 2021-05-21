namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class UserNameChangeMessageComposer : ServerPacket
    {
        public UserNameChangeMessageComposer(string Name, int VirtualId)
            : base(ServerPacketHeader.UNIT_CHANGE_NAME)
        {
            this.WriteInteger(0);
            this.WriteInteger(VirtualId);
            this.WriteString(Name);
        }
    }
}
