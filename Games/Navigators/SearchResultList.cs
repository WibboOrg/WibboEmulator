namespace WibboEmulator.Games.Navigators;

public class SearchResultList(int id, string category, string categoryIdentifier, string publicName, bool canDoActions, int colour, int requiredRank,
    bool minimized, NavigatorViewMode viewMode, string categoryType, string searchAllowance, int orderId)
{
    public int Id { get; set; } = id;

    public string Category { get; set; } = category;

    public string CategoryIdentifier { get; set; } = categoryIdentifier;

    public string PublicName { get; set; } = publicName;

    public bool Minimized { get; set; } = minimized;

    public bool CanDoActions { get; set; } = canDoActions;

    public int Colour { get; set; } = colour;

    public int RequiredRank { get; set; } = requiredRank;

    public NavigatorViewMode ViewMode { get; set; } = viewMode;

    public NavigatorCategoryType CategoryType { get; set; } = NavigatorCategoryTypeUtility.GetCategoryTypeByString(categoryType);

    public NavigatorSearchAllowance SearchAllowance { get; set; } = NavigatorSearchAllowanceUtility.GetSearchAllowanceByString(searchAllowance);

    public int OrderId { get; set; } = orderId;
}
