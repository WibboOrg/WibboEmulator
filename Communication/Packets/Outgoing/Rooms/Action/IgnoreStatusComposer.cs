namespace Butterfly.Communication.Packets.Outgoing.Rooms.Action
{
    internal class IgnoreStatusComposer : ServerPacket
    {
        public IgnoreStatusComposer(int Statue, string Name)
            : base(ServerPacketHeader.USER_IGNORED_RESULT)
        {
            this.WriteInteger(Statue);
            this.WriteString(Name);
        }
    }
}
