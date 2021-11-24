namespace Butterfly.Communication.Packets.Outgoing.LandingView
{
    internal class PromoArticlesComposer : ServerPacket
    {
        public PromoArticlesComposer()
            : base(ServerPacketHeader.DESKTOP_NEWS)
        {

        }
    }
}
