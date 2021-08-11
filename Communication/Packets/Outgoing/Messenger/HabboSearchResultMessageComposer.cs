namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class HabboSearchResultMessageComposer : ServerPacket
    {
        public HabboSearchResultMessageComposer()
            : base(ServerPacketHeader.MESSENGER_SEARCH)
        {

        }
    }
}
