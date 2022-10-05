namespace WibboEmulator.Games.Navigator;

public class SearchResultList
{
    private int _orderId;

    public SearchResultList(int Id, string Category, string CategoryIdentifier, string PublicName, bool CanDoActions, int Colour, int RequiredRank, bool Minimized, NavigatorViewMode ViewMode, string CategoryType, string SearchAllowance, int OrderId)
    {
        this.Id = Id;
        this.Category = Category;
        this.CategoryIdentifier = CategoryIdentifier;
        this.PublicName = PublicName;
        this.CanDoActions = CanDoActions;
        this.Colour = Colour;
        this.RequiredRank = RequiredRank;
        this.ViewMode = ViewMode;
        this.Minimized = Minimized;
        this.CategoryType = NavigatorCategoryTypeUtility.GetCategoryTypeByString(CategoryType);
        this.SearchAllowance = NavigatorSearchAllowanceUtility.GetSearchAllowanceByString(SearchAllowance);
        this._orderId = OrderId;
    }

    public int Id { get; set; }

    public string Category { get; set; }

    public string CategoryIdentifier { get; set; }

    public string PublicName { get; set; }

    public bool Minimized { get; set; }

    public bool CanDoActions { get; set; }

    public int Colour { get; set; }

    public int RequiredRank { get; set; }

    public NavigatorViewMode ViewMode { get; set; }

    public NavigatorCategoryType CategoryType { get; set; }

    public NavigatorSearchAllowance SearchAllowance { get; set; }

    public int OrderId
    {
        get => this._orderId;
        set => this._orderId = value;
    }
}
