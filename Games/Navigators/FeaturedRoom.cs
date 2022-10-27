namespace WibboEmulator.Games.Navigators;
using WibboEmulator.Core.Language;

public class FeaturedRoom
{
    public int RoomId { get; private set; }
    public string Image { get; private set; }
    public Language Langue { get; private set; }
    public NavigatorCategoryType CategoryType { get; private set; }

    public FeaturedRoom(int roomId, string image, Language langue, string categoryType)
    {
        this.RoomId = roomId;
        this.Image = image;
        this.Langue = langue;
        this.CategoryType = NavigatorCategoryTypeUtility.GetCategoryTypeByString(categoryType);
    }
}
