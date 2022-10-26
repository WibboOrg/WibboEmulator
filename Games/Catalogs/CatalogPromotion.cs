namespace WibboEmulator.Games.Catalogs;
using WibboEmulator.Core.Language;

public class CatalogPromotion
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public string TitleBr { get; set; }
    public string Image { get; set; }
    public int Unknown { get; set; }
    public string PageLink { get; set; }
    public int ParentId { get; set; }

    public CatalogPromotion(int id, string title, string titleEn, string titleBr, string image, int unknown, string pageLink, int parentId)
    {
        this.Id = id;
        this.Title = title;
        this.TitleEn = titleEn;
        this.TitleBr = titleBr;
        this.Image = image;
        this.Unknown = unknown;
        this.PageLink = pageLink;
        this.ParentId = parentId;
    }

    public string GetTitleByLangue(Language langue)
    {
        if (langue == Language.English)
        {
            return this.TitleEn;
        }
        else if (langue == Language.Portuguese)
        {
            return this.TitleBr;
        }

        return this.Title;
    }
}
