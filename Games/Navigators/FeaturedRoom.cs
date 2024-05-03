namespace WibboEmulator.Games.Navigators;
using WibboEmulator.Core.Language;

public class FeaturedRoom(int roomId, string image, Language langue, string categoryType)
{
    public int RoomId { get; private set; } = roomId;
    public string Image { get; private set; } = image;
    public Language Langue { get; private set; } = langue;
    public NavigatorCategoryType CategoryType { get; private set; } = NavigatorCategoryTypeUtility.GetCategoryTypeByString(categoryType);
}
