namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class HabboSearchResultComposer : ServerPacket
    {
        public HabboSearchResultComposer()
            : base(ServerPacketHeader.MESSENGER_SEARCH)
        {

        }
    }
}
