namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class GetRelationshipsComposer : ServerPacket
    {
        public GetRelationshipsComposer()
            : base(ServerPacketHeader.MESSENGER_RELATIONSHIPS)
        {

        }
    }
}
