namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;

internal sealed class CatalogIndexComposer : ServerPacket
{
    public CatalogIndexComposer(GameClient Session, ICollection<CatalogPage> pages)
         : base(ServerPacketHeader.CATALOG_PAGE_LIST)
    {
        this.WriteRootIndex(Session, pages);

        foreach (var parent in pages)
        {
            if (parent.ParentId != -1 || !parent.HavePermission(Session.User))
            {
                continue;
            }

            this.WritePage(parent, CalcTreeSize(Session, pages, parent.Id), Session.Language);

            foreach (var child in pages)
            {
                if (child.ParentId != parent.Id || !child.HavePermission(Session.User))
                {
                    continue;
                }

                if (child.Enabled)
                {
                    this.WritePage(child, CalcTreeSize(Session, pages, child.Id), Session.Language);
                }
                else
                {
                    this.WriteNodeIndex(child, CalcTreeSize(Session, pages, child.Id), Session.Language);
                }

                foreach (var subChild in pages)
                {
                    if (subChild.ParentId != child.Id || !subChild.HavePermission(Session.User))
                    {
                        continue;
                    }

                    if (subChild.Enabled)
                    {
                        this.WritePage(subChild, CalcTreeSize(Session, pages, subChild.Id), Session.Language);
                    }
                    else
                    {
                        this.WriteNodeIndex(subChild, CalcTreeSize(Session, pages, subChild.Id), Session.Language);
                    }

                    foreach (var subSubChild in pages)
                    {
                        if (subSubChild.ParentId != subChild.Id || !subSubChild.HavePermission(Session.User))
                        {
                            continue;
                        }

                        if (subSubChild.Enabled)
                        {
                            this.WritePage(subSubChild, 0, Session.Language);
                        }
                        else
                        {
                            this.WriteNodeIndex(subSubChild, 0, Session.Language);
                        }
                    }
                }
            }
        }

        this.WriteBoolean(false);
        this.WriteString("NORMAL");
    }

    public void WriteRootIndex(GameClient Session, ICollection<CatalogPage> pages)
    {
        this.WriteBoolean(true);
        this.WriteInteger(0);
        this.WriteInteger(-1);
        this.WriteString("root");
        this.WriteString(string.Empty);

        this.WriteInteger(0);
        this.WriteInteger(CalcTreeSize(Session, pages, -1));
    }

    public void WriteNodeIndex(CatalogPage page, int treeSize, Language langue)
    {
        this.WriteBoolean(true); // Visible
        this.WriteInteger(page.Icon);
        this.WriteInteger(-1);
        this.WriteString(page.PageLink);
        this.WriteString(page.GetCaptionByLangue(langue));
        this.WriteInteger(0);
        this.WriteInteger(treeSize);
    }

    public void WritePage(CatalogPage page, int treeSize, Language langue)
    {
        this.WriteBoolean(true);
        this.WriteInteger(page.Icon);
        this.WriteInteger(page.Id);
        this.WriteString(page.PageLink);
        this.WriteString(page.GetCaptionByLangue(langue));

        this.WriteInteger(page.ItemOffers.Count);
        foreach (var i in page.ItemOffers.Keys)
        {
            this.WriteInteger(i);
        }

        this.WriteInteger(treeSize);
    }

    public static int CalcTreeSize(GameClient Session, ICollection<CatalogPage> pages, int parentId)
    {
        var i = 0;
        foreach (var page in pages)
        {
            if (!page.HavePermission(Session.User) || page.ParentId != parentId)
            {
                continue;
            }

            if (page.ParentId == parentId)
            {
                i++;
            }
        }

        return i;
    }
}
