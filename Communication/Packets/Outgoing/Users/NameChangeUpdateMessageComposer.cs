namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class NameChangeUpdateMessageComposer : ServerPacket
    {
        public NameChangeUpdateMessageComposer()
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {

        }
    }
}
