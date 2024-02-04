namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Catalogs;

public class CatalogPageComposer : ServerPacket
{
    public CatalogPageComposer(CatalogPage page, string cataMode, Language langue, int offerId = -1)
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
                CatalogItemUtility.GenerateOfferData(item, page.IsPremium, this);
            }
        }
        else
        {
            this.WriteInteger(0);
        }

        this.WriteInteger(offerId);
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
