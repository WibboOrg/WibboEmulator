using System.Collections.Generic;
using System.Linq;
using Butterfly.HabboHotel.LandingView.Promotions;

namespace Butterfly.Communication.Packets.Outgoing.LandingView
{
    internal class PromoArticlesMessageComposer : ServerPacket
    {
        public PromoArticlesMessageComposer(ICollection<Promotion> LandingPromotions)
            : base(ServerPacketHeader.DESKTOP_NEWS)
        {
            WriteInteger(LandingPromotions.Count);//Count
            foreach (Promotion Promotion in LandingPromotions.ToList())
            {
                WriteInteger(Promotion.Id); //ID
                WriteString(Promotion.Title); //Title
                WriteString(Promotion.Text); //Text
                WriteString(Promotion.ButtonText); //Button text
                WriteInteger(Promotion.ButtonType); //Link type 0 and 3
                WriteString(Promotion.ButtonLink); //Link to article
                WriteString(Promotion.ImageLink); //Image link
            }
        }
    }
}

