using Wibbo.Core;

namespace Wibbo.Game.Catalog
{
    public class CatalogPromotion
    {
        public int Id;
        public string Title;
        public string TitleEn;
        public string TitleBr;
        public string Image;
        public int Unknown;
        public string PageLink;
        public int ParentId;

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
            if (langue == Language.ANGLAIS)
            {
                return this.TitleEn;
            }
            else if (langue == Language.PORTUGAIS)
            {
                return this.TitleBr;
            }

            return this.Title;
        }
    }
}
