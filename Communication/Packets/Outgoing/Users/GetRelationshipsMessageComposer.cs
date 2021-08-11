namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class GetRelationshipsMessageComposer : ServerPacket
    {
        public GetRelationshipsMessageComposer()
            : base(ServerPacketHeader.MESSENGER_RELATIONSHIPS)
        {

        }
    }
}
