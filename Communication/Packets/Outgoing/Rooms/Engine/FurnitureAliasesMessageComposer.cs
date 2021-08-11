namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FurnitureAliasesMessageComposer : ServerPacket
    {
        public FurnitureAliasesMessageComposer()
            : base(ServerPacketHeader.FURNITURE_ALIASES)
        {

        }
    }
}
