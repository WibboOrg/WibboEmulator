namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.Items;

public class CatalogPageComposer : ServerPacket
{
    public CatalogPageComposer(CatalogPage page, string cataMode, Language langue)
        : base(ServerPacketHeader.CATALOG_PAGE)
    {
        this.WriteInteger(page.Id);
        this.WriteString(cataMode);
        this.WriteString(page.Template);

        this.WriteInteger(page.PageStrings1.Count);
        foreach (var s in page.PageStrings1)
        {
            this.WriteString(s);
        }

        if (page.GetPageStrings2ByLangue(langue).Count == 1 && (page.Template == "default_3x3" || page.Template == "default_3x3_color_grouping") && string.IsNullOrEmpty(page.GetPageStrings2ByLangue(langue)[0]))
        {
            this.WriteInteger(1);
            this.WriteString(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("catalog.desc.default", langue), page.GetCaptionByLangue(langue)));
        }
        else
        {
            this.WriteInteger(page.GetPageStrings2ByLangue(langue).Count);
            foreach (var s in page.GetPageStrings2ByLangue(langue))
            {
                this.WriteString(s);
            }
        }

        if (!page.Template.Equals("frontpage") && !page.Template.Equals("club_buy"))
        {
            this.WriteInteger(page.Items.Count);
            foreach (var item in page.Items.Values)
            {
                this.WriteInteger(item.Id);
                this.WriteString(item.Name);
                this.WriteBoolean(false);//IsRentable
                this.WriteInteger(item.CostCredits);

                if (item.CostWibboPoints > 0)
                {
                    this.WriteInteger(item.CostWibboPoints);
                    this.WriteInteger(105); // Diamonds
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
        else
        {
            this.WriteInteger(0);
        }

        this.WriteInteger(-1);
        this.WriteBoolean(false);

        this.WriteInteger(WibboEnvironment.GetGame().GetCatalog().GetPromotions().ToList().Count);//Count
        foreach (var promotion in WibboEnvironment.GetGame().GetCatalog().GetPromotions().ToList())
        {
            this.WriteInteger(promotion.Id);
            this.WriteString(promotion.GetTitleByLangue(langue));
            this.WriteString(promotion.Image);
            this.WriteInteger(promotion.Unknown);
            this.WriteString(promotion.PageLink);
            this.WriteInteger(promotion.ParentId);
        }
    }
}
