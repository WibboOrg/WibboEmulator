namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class HabboSearchResultMessageComposer : ServerPacket
    {
        public HabboSearchResultMessageComposer()
            : base(ServerPacketHeader.MESSENGER_SEARCH)
        {

        }
    }
}
