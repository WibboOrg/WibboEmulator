namespace Butterfly.Communication.Packets.Outgoing.LandingView
{
    internal class PromoArticlesMessageComposer : ServerPacket
    {
        public PromoArticlesMessageComposer()
            : base(ServerPacketHeader.DESKTOP_NEWS)
        {

        }
    }
}
