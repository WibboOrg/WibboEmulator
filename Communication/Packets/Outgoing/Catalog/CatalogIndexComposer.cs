namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;

internal sealed class CatalogIndexComposer : ServerPacket
{
    public CatalogIndexComposer(GameClient session, ICollection<CatalogPage> pages)
         : base(ServerPacketHeader.CATALOG_PAGE_LIST)
    {
        this.WriteRootIndex(session, pages);

        foreach (var parent in pages)
        {
            if (parent.ParentId != -1 || !parent.HavePermission(session.User))
            {
                continue;
            }

            this.WritePage(parent, CalcTreeSize(session, pages, parent.Id), session.Language);

            foreach (var child in pages)
            {
                if (child.ParentId != parent.Id || !child.HavePermission(session.User))
                {
                    continue;
                }

                if (child.Enabled)
                {
                    this.WritePage(child, CalcTreeSize(session, pages, child.Id), session.Language);
                }
                else
                {
                    this.WriteNodeIndex(child, CalcTreeSize(session, pages, child.Id), session.Language);
                }

                foreach (var subChild in pages)
                {
                    if (subChild.ParentId != child.Id || !subChild.HavePermission(session.User))
                    {
                        continue;
                    }

                    if (subChild.Enabled)
                    {
                        this.WritePage(subChild, CalcTreeSize(session, pages, subChild.Id), session.Language);
                    }
                    else
                    {
                        this.WriteNodeIndex(subChild, CalcTreeSize(session, pages, subChild.Id), session.Language);
                    }

                    foreach (var subSubChild in pages)
                    {
                        if (subSubChild.ParentId != subChild.Id || !subSubChild.HavePermission(session.User))
                        {
                            continue;
                        }

                        if (subSubChild.Enabled)
                        {
                            this.WritePage(subSubChild, 0, session.Language);
                        }
                        else
                        {
                            this.WriteNodeIndex(subSubChild, 0, session.Language);
                        }
                    }
                }
            }
        }

        this.WriteBoolean(false);
        this.WriteString("NORMAL");
    }

    public void WriteRootIndex(GameClient session, ICollection<CatalogPage> pages)
    {
        this.WriteBoolean(true);
        this.WriteInteger(0);
        this.WriteInteger(-1);
        this.WriteString("root");
        this.WriteString(string.Empty);

        this.WriteInteger(0);
        this.WriteInteger(CalcTreeSize(session, pages, -1));
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

    public static int CalcTreeSize(GameClient session, ICollection<CatalogPage> pages, int parentId)
    {
        var i = 0;
        foreach (var page in pages)
        {
            if (!page.HavePermission(session.User) || page.ParentId != parentId)
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
