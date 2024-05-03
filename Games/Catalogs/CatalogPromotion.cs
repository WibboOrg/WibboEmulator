namespace WibboEmulator.Games.Catalogs;
using WibboEmulator.Core.Language;

public class CatalogPromotion(int id, string title, string titleEn, string titleBr, string image, int unknown, string pageLink, int parentId)
{
    public int Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string TitleEn { get; set; } = titleEn;
    public string TitleBr { get; set; } = titleBr;
    public string Image { get; set; } = image;
    public int Unknown { get; set; } = unknown;
    public string PageLink { get; set; } = pageLink;
    public int ParentId { get; set; } = parentId;

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
