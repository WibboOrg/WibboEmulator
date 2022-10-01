using WibboEmulator.Core;

namespace WibboEmulator.Games.Navigator
{
    public class FeaturedRoom
    {
        public int RoomId { get; private set; }
        public string Image { get; private set; }
        public Language Langue { get; private set; }
        public NavigatorCategoryType CategoryType { get; private set; }
        public FeaturedRoom(int RoomId, string Image, Language Langue, string CategoryType)
        {
            this.RoomId = RoomId;
            this.Image = Image;
            this.Langue = Langue;
            this.CategoryType = NavigatorCategoryTypeUtility.GetCategoryTypeByString(CategoryType);
        }
    }
}