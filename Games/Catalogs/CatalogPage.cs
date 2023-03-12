namespace WibboEmulator.Games.Catalogs;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Users;
using WibboEmulator.WebSocketSharp;

public class CatalogPage
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public bool Enabled { get; set; }
    public bool IsPremium { get; set; }
    public string Caption { get; set; }
    public string CaptionEn { get; set; }
    public string CaptionBr { get; set; }
    public string PageLink { get; set; }
    public int Icon { get; set; }
    public string RequiredRight { get; set; }
    public string Template { get; set; }
    public List<string> PageStrings1 { get; private set; }
    public Dictionary<int, CatalogItem> Items { get; private set; }
    public Dictionary<int, CatalogItem> ItemOffers { get; private set; }

    private readonly List<string> _pageStrings2;
    private readonly List<string> _pageStrings2En;
    private readonly List<string> _pageStrings2Br;

    public CatalogPage(int id, int parentId, string enabled, string caption, string pageLink, int icon, string requiredRight,
        string template, string pageStrings1, string pageStrings2, string captionEn, string captionBr, string pageStrings2En,
        string pageStrings2Br, string isPremium, Dictionary<int, CatalogItem> items)
    {
        this.Id = id;
        this.ParentId = parentId;
        this.Enabled = enabled.ToLower() == "1";
        this.IsPremium = isPremium.ToLower() == "1";

        this.Caption = caption;
        this.CaptionEn = captionEn;
        this.CaptionBr = captionBr;

        this.PageLink = pageLink;
        this.Icon = icon;
        this.RequiredRight = requiredRight;
        this.Template = template;

        foreach (var str in pageStrings1.Split('|'))
        {
            this.PageStrings1 ??= new List<string>();
            this.PageStrings1.Add(str);
        }

        foreach (var str in pageStrings2.Split('|'))
        {
            this._pageStrings2 ??= new List<string>();
            this._pageStrings2.Add(str);
        }

        foreach (var str in pageStrings2En.Split('|'))
        {
            this._pageStrings2En ??= new List<string>();
            this._pageStrings2En.Add(str);
        }

        foreach (var str in pageStrings2Br.Split('|'))
        {
            this._pageStrings2Br ??= new List<string>();
            this._pageStrings2Br.Add(str);
        }

        this.Items = items;

        this.ItemOffers = new Dictionary<int, CatalogItem>();
        if (template.StartsWith("default_3x3"))
        {
            foreach (var item in this.Items.Values)
            {
                if (item.IsLimited)
                {
                    continue;
                }

                if (!this.ItemOffers.ContainsKey(item.Id))
                {
                    this.ItemOffers.Add(item.Id, item);
                }
            }
        }
    }

    public string GetCaptionByLangue(Language langue)
    {
        if (langue == Language.English)
        {
            return this.CaptionEn;
        }
        else if (langue == Language.Portuguese)
        {
            return this.CaptionBr;
        }

        return this.Caption;
    }

    public List<string> GetPageStrings2ByLangue(Language langue)
    {
        if (langue == Language.English)
        {
            return this._pageStrings2En;
        }
        else if (langue == Language.Portuguese)
        {
            return this._pageStrings2Br;
        }

        return this._pageStrings2;
    }

    public CatalogItem GetItem(int id)
    {
        if (this.Items.TryGetValue(id, out var value))
        {
            return value;
        }

        return null;
    }

    public bool HavePermission(User user) => this.RequiredRight.IsNullOrEmpty() || user.HasPermission(this.RequiredRight);
}
