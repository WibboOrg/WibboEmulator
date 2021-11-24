namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class NameChangeUpdateComposer : ServerPacket
    {
        public NameChangeUpdateComposer()
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {

        }
    }
}
