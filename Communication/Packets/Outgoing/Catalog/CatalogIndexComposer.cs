using Wibbo.Core;
using Wibbo.Game.Catalog;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(Client Session, ICollection<CatalogPage> Pages, int Sub = 0)
             : base(ServerPacketHeader.CATALOG_PAGE_LIST)
        {
            this.WriteRootIndex(Session, Pages);

            foreach (CatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || Parent.MinimumRank > Session.GetUser().Rank)
                {
                    continue;
                }

                this.WritePage(Parent, this.CalcTreeSize(Session, Pages, Parent.Id), Session.Langue);

                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != Parent.Id || child.MinimumRank > Session.GetUser().Rank)
                    {
                        continue;
                    }

                    if (child.Enabled)
                    {
                        this.WritePage(child, this.CalcTreeSize(Session, Pages, child.Id), Session.Langue);
                    }
                    else
                    {
                        this.WriteNodeIndex(child, this.CalcTreeSize(Session, Pages, child.Id), Session.Langue);
                    }

                    foreach (CatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != child.Id || SubChild.MinimumRank > Session.GetUser().Rank)
                        {
                            continue;
                        }

                        if (SubChild.Enabled)
                        {
                            this.WritePage(SubChild, this.CalcTreeSize(Session, Pages, SubChild.Id), Session.Langue);
                        }
                        else
                        {
                            this.WriteNodeIndex(SubChild, this.CalcTreeSize(Session, Pages, SubChild.Id), Session.Langue);
                        }

                        foreach (CatalogPage SubSubChild in Pages)
                        {
                            if (SubSubChild.ParentId != SubChild.Id || SubSubChild.MinimumRank > Session.GetUser().Rank)
                            {
                                continue;
                            }

                            if (SubSubChild.Enabled)
                            {
                                this.WritePage(SubSubChild, 0, Session.Langue);
                            }
                            else
                            {
                                this.WriteNodeIndex(SubSubChild, 0, Session.Langue);
                            }
                        }
                    }
                }
            }

            this.WriteBoolean(false);
            this.WriteString("NORMAL");
        }

        public void WriteRootIndex(Client session, ICollection<CatalogPage> pages)
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
            foreach (int i in page.ItemOffers.Keys)
            {
                this.WriteInteger(i);
            }

            this.WriteInteger(treeSize);
        }

        public int CalcTreeSize(Client Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Session.GetUser().Rank || Page.ParentId != ParentId)
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
}