namespace WibboEmulator.Communication.Packets.Outgoing.LandingView;

using WibboEmulator.Games.LandingView;

internal sealed class PromoArticlesComposer : ServerPacket
{
    public PromoArticlesComposer(List<Promotion> hotelViewPromosIndexers)
        : base(ServerPacketHeader.DESKTOP_NEWS)
    {
        this.WriteInteger(hotelViewPromosIndexers.Count);
        foreach (var promo in hotelViewPromosIndexers)
        {
            this.WriteInteger(promo.Index);
            this.WriteString(promo.Header);
            this.WriteString(promo.Body);
            this.WriteString(promo.Button);
            this.WriteInteger(promo.InGamePromo);
            this.WriteString(promo.SpecialAction);
            this.WriteString(promo.Image);
        }
    }
}
