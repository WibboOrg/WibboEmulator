namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class NameChangeUpdateMessageComposer : ServerPacket
    {
        public NameChangeUpdateMessageComposer()
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {

        }
    }
}
