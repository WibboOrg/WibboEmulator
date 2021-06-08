using Butterfly.Core;
using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.Items;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    public class CatalogPageComposer : ServerPacket
    {
        public CatalogPageComposer(CatalogPage Page, string CataMode, Language Langue)
            : base(ServerPacketHeader.CATALOG_PAGE)
        {
            this.WriteInteger(Page.Id);
            this.WriteString(CataMode);
            this.WriteString(Page.Template);

            this.WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                this.WriteString(s);
            }

            if (Page.GetPageStrings2ByLangue(Langue).Count == 1 && (Page.Template == "default_3x3" || Page.Template == "default_3x3_color_grouping") && string.IsNullOrEmpty(Page.GetPageStrings2ByLangue(Langue)[0]))
            {
                this.WriteInteger(1);
                this.WriteString(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("catalog.desc.default", Langue), Page.GetCaptionByLangue(Langue)));
            }
            else
            {
                this.WriteInteger(Page.GetPageStrings2ByLangue(Langue).Count);
                foreach (string s in Page.GetPageStrings2ByLangue(Langue))
                {
                    this.WriteString(s);
                }
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy"))
            {
                this.WriteInteger(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    this.WriteInteger(Item.Id);
                    this.WriteString(Item.Name);
                    this.WriteBoolean(false);//IsRentable
                    this.WriteInteger(Item.CostCredits);

                    if (Item.CostDiamonds > 0)
                    {
                        this.WriteInteger(Item.CostDiamonds);
                        this.WriteInteger(105); // Diamonds
                    }
                    else
                    {
                        this.WriteInteger(Item.CostDuckets);
                        this.WriteInteger(0);
                    }

                    this.WriteBoolean(ItemUtility.CanGiftItem(Item));

                    this.WriteInteger(string.IsNullOrEmpty(Item.Badge) || Item.Data.Type.ToString() == "b" ? 1 : 2);

                    if (Item.Data.Type.ToString().ToLower() != "b")
                    {
                        this.WriteString(Item.Data.Type.ToString());
                        this.WriteInteger(Item.Data.SpriteId);
                        if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                        {
                            this.WriteString(Item.Name.Split('_')[2]);
                        }
                        else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                        {
                            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot CatalogBot))
                            {
                                this.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                            }
                            else
                            {
                                this.WriteString(CatalogBot.Figure);
                            }
                        }
                        else
                        {
                            this.WriteString("");
                        }
                        this.WriteInteger(Item.Amount);
                        this.WriteBoolean(Item.IsLimited); // IsLimited
                        if (Item.IsLimited)
                        {
                            this.WriteInteger(Item.LimitedEditionStack);
                            this.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                        }
                    }

                    if (!string.IsNullOrEmpty(Item.Badge))
                    {
                        this.WriteString("b");
                        this.WriteString(Item.Badge);
                    }

                    this.WriteInteger(0); //club_level
                    this.WriteBoolean(ItemUtility.CanSelectAmount(Item));

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

            this.WriteInteger(ButterflyEnvironment.GetGame().GetCatalog().GetPromotions().ToList().Count);//Count
            foreach (CatalogPromotion Promotion in ButterflyEnvironment.GetGame().GetCatalog().GetPromotions().ToList())
            {
                this.WriteInteger(Promotion.Id);
                this.WriteString(Promotion.GetTitleByLangue(Langue));
                this.WriteString(Promotion.Image);
                this.WriteInteger(Promotion.Unknown);
                this.WriteString(Promotion.PageLink);
                this.WriteInteger(Promotion.ParentId);
            }
        }
    }
}