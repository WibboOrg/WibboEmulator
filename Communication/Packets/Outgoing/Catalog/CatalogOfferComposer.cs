using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogOfferComposer : ServerPacket
    {
        public CatalogOfferComposer(CatalogItem Item)
            : base(ServerPacketHeader.CATALOG_SEARCH)
        {
            this.WriteInteger(Item.Id);
            this.WriteString(Item.Name);
            this.WriteBoolean(false);//IsRentable
            this.WriteInteger(Item.CostCredits);

            if (Item.CostWibboPoints > 0)
            {
                this.WriteInteger(Item.CostWibboPoints);
                this.WriteInteger(105); // Diamonds
            }
            else
            {
                this.WriteInteger(Item.CostDuckets);
                this.WriteInteger(0);
            }

            this.WriteBoolean(ItemUtility.CanGiftItem(Item));

            this.WriteInteger(string.IsNullOrEmpty(Item.Badge) || Item.Data.Type.ToString() == "b" ? 1 : 2);

            if (Item.Data.Type.ToString().ToLower() != "b")
            {
                this.WriteString(Item.Data.Type.ToString());
                this.WriteInteger(Item.Data.SpriteId);
                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                {
                    this.WriteString(Item.Name.Split('_')[2]);
                }
                else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                {
                    if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot CatalogBot))
                    {
                        this.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    }
                    else
                    {
                        this.WriteString(CatalogBot.Figure);
                    }
                }
                else
                {
                    this.WriteString("");
                }
                this.WriteInteger(Item.Amount);
                this.WriteBoolean(Item.IsLimited); // IsLimited
                if (Item.IsLimited)
                {
                    this.WriteInteger(Item.LimitedEditionStack);
                    this.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                }
            }

            if (!string.IsNullOrEmpty(Item.Badge))
            {
                this.WriteString("b");
                this.WriteString(Item.Badge);
            }

            this.WriteInteger(0); //club_level
            this.WriteBoolean(ItemUtility.CanSelectAmount(Item));

            this.WriteBoolean(false);// TODO: Figure out
            this.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
        }
    }
}
