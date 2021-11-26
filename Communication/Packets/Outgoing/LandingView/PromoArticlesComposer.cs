using Butterfly.Game.LandingView;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.LandingView
{
    internal class PromoArticlesComposer : ServerPacket
    {
        public PromoArticlesComposer(List<Promotion> hotelViewPromosIndexers)
            : base(ServerPacketHeader.DESKTOP_NEWS)
        {
            this.WriteInteger(hotelViewPromosIndexers.Count);
            foreach (Promotion promo in hotelViewPromosIndexers)
            {
                this.WriteInteger(promo.Index);
                this.WriteString(promo.Header);
                this.WriteString(promo.Body);
                this.WriteString(promo.Button);
                this.WriteInteger(promo.inGamePromo);
                this.WriteString(promo.SpecialAction);
                this.WriteString(promo.Image);
            }
        }
    }
}
