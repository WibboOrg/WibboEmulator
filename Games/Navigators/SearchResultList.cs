namespace WibboEmulator.Games.Navigators;

public class SearchResultList
{
    public SearchResultList(int id, string category, string categoryIdentifier, string publicName, bool canDoActions, int colour, int requiredRank,
        bool minimized, NavigatorViewMode viewMode, string categoryType, string searchAllowance, int orderId)
    {
        this.Id = id;
        this.Category = category;
        this.CategoryIdentifier = categoryIdentifier;
        this.PublicName = publicName;
        this.CanDoActions = canDoActions;
        this.Colour = colour;
        this.RequiredRank = requiredRank;
        this.ViewMode = viewMode;
        this.Minimized = minimized;
        this.CategoryType = NavigatorCategoryTypeUtility.GetCategoryTypeByString(categoryType);
        this.SearchAllowance = NavigatorSearchAllowanceUtility.GetSearchAllowanceByString(searchAllowance);
        this.OrderId = orderId;
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

    public int OrderId { get; set; }
}
