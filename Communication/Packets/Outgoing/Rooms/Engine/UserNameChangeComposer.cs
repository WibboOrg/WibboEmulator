namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserNameChangeComposer : ServerPacket
    {
        public UserNameChangeComposer(string Name, int VirtualId)
            : base(ServerPacketHeader.UNIT_CHANGE_NAME)
        {
            this.WriteInteger(0);
            this.WriteInteger(VirtualId);
            this.WriteString(Name);
        }
    }
}
