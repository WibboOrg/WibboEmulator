namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FurnitureAliasesMessageComposer : ServerPacket
    {
        public FurnitureAliasesMessageComposer()
            : base(ServerPacketHeader.FURNITURE_ALIASES)
        {

        }
    }
}
