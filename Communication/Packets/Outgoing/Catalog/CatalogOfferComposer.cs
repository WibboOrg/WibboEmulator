namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalog;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.Items;

internal class CatalogOfferComposer : ServerPacket
{
    public CatalogOfferComposer(CatalogItem item)
        : base(ServerPacketHeader.PRODUCT_OFFER)
    {
        this.WriteInteger(item.Id);
        this.WriteString(item.Name);
        this.WriteBoolean(false);//IsRentable
        this.WriteInteger(item.CostCredits);

        if (item.CostWibboPoints > 0)
        {
            this.WriteInteger(item.CostWibboPoints);
            this.WriteInteger(105);
        }
        else if (item.CostLimitCoins > 0)
        {
            this.WriteInteger(item.CostLimitCoins);
            this.WriteInteger(55);
        }
        else
        {
            this.WriteInteger(item.CostDuckets);
            this.WriteInteger(0);
        }

        this.WriteBoolean(ItemUtility.CanGiftItem(item));

        this.WriteInteger(string.IsNullOrEmpty(item.Badge) || item.Data.Type.ToString() == "b" ? 1 : 2);

        if (item.Data.Type.ToString().ToLower() != "b")
        {
            this.WriteString(item.Data.Type.ToString());
            this.WriteInteger(item.Data.SpriteId);
            if (item.Data.InteractionType is InteractionType.WALLPAPER or InteractionType.FLOOR or InteractionType.LANDSCAPE)
            {
                this.WriteString(item.Name.Split('_')[2]);
            }
            else if (item.Data.InteractionType == InteractionType.BOT)//Bots
            {
                if (!WibboEnvironment.GetGame().GetCatalog().TryGetBot(item.ItemId, out var catalogBot))
                {
                    this.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                }
                else
                {
                    this.WriteString(catalogBot.Figure);
                }
            }
            else
            {
                this.WriteString("");
            }
            this.WriteInteger(item.Amount);
            this.WriteBoolean(item.IsLimited); // IsLimited
            if (item.IsLimited)
            {
                this.WriteInteger(item.LimitedEditionStack);
                this.WriteInteger(item.LimitedEditionStack - item.LimitedEditionSells);
            }
        }

        if (!string.IsNullOrEmpty(item.Badge))
        {
            this.WriteString("b");
            this.WriteString(item.Badge);
        }

        this.WriteInteger(0); //club_level
        this.WriteBoolean(ItemUtility.CanSelectAmount(item));

        this.WriteBoolean(false);// TODO: Figure out
        this.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
    }
}
