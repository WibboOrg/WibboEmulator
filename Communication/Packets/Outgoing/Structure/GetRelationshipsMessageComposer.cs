namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class GetRelationshipsMessageComposer : ServerPacket
    {
        public GetRelationshipsMessageComposer()
            : base(ServerPacketHeader.MESSENGER_RELATIONSHIPS)
        {

        }
    }
}
