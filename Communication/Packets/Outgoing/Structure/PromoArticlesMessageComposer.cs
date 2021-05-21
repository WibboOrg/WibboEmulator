namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class PromoArticlesMessageComposer : ServerPacket
    {
        public PromoArticlesMessageComposer()
            : base(ServerPacketHeader.DESKTOP_NEWS)
        {

        }
    }
}
