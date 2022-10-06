namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Catalog;
using WibboEmulator.Games.GameClients;

internal class CatalogIndexComposer : ServerPacket
{
    public CatalogIndexComposer(GameClient session, ICollection<CatalogPage> Pages, int Sub = 0)
         : base(ServerPacketHeader.CATALOG_PAGE_LIST)
    {
        this.WriteRootIndex(session, Pages);

        foreach (var Parent in Pages)
        {
            if (Parent.ParentId != -1 || Parent.MinimumRank > session.GetUser().Rank)
            {
                continue;
            }

            this.WritePage(Parent, this.CalcTreeSize(session, Pages, Parent.Id), session.Langue);

            foreach (var child in Pages)
            {
                if (child.ParentId != Parent.Id || child.MinimumRank > session.GetUser().Rank)
                {
                    continue;
                }

                if (child.Enabled)
                {
                    this.WritePage(child, this.CalcTreeSize(session, Pages, child.Id), session.Langue);
                }
                else
                {
                    this.WriteNodeIndex(child, this.CalcTreeSize(session, Pages, child.Id), session.Langue);
                }

                foreach (var SubChild in Pages)
                {
                    if (SubChild.ParentId != child.Id || SubChild.MinimumRank > session.GetUser().Rank)
                    {
                        continue;
                    }

                    if (SubChild.Enabled)
                    {
                        this.WritePage(SubChild, this.CalcTreeSize(session, Pages, SubChild.Id), session.Langue);
                    }
                    else
                    {
                        this.WriteNodeIndex(SubChild, this.CalcTreeSize(session, Pages, SubChild.Id), session.Langue);
                    }

                    foreach (var SubSubChild in Pages)
                    {
                        if (SubSubChild.ParentId != SubChild.Id || SubSubChild.MinimumRank > session.GetUser().Rank)
                        {
                            continue;
                        }

                        if (SubSubChild.Enabled)
                        {
                            this.WritePage(SubSubChild, 0, session.Langue);
                        }
                        else
                        {
                            this.WriteNodeIndex(SubSubChild, 0, session.Langue);
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
        this.WriteInteger(this.CalcTreeSize(session, pages, -1));
    }

    public void WriteNodeIndex(CatalogPage page, int treeSize, Language Langue)
    {
        this.WriteBoolean(true); // Visible
        this.WriteInteger(page.Icon);
        this.WriteInteger(-1);
        this.WriteString(page.PageLink);
        this.WriteString(page.GetCaptionByLangue(Langue));
        this.WriteInteger(0);
        this.WriteInteger(treeSize);
    }

    public void WritePage(CatalogPage page, int treeSize, Language Langue)
    {
        this.WriteBoolean(true);
        this.WriteInteger(page.Icon);
        this.WriteInteger(page.Id);
        this.WriteString(page.PageLink);
        this.WriteString(page.GetCaptionByLangue(Langue));

        this.WriteInteger(page.ItemOffers.Count);
        foreach (var i in page.ItemOffers.Keys)
        {
            this.WriteInteger(i);
        }

        this.WriteInteger(treeSize);
    }

    public int CalcTreeSize(GameClient session, ICollection<CatalogPage> Pages, int ParentId)
    {
        var i = 0;
        foreach (var Page in Pages)
        {
            if (Page.MinimumRank > session.GetUser().Rank || Page.ParentId != ParentId)
            {
                continue;
            }

            if (Page.ParentId == ParentId)
            {
                i++;
            }
        }

        return i;
    }
}