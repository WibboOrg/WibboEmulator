namespace Butterfly.Game.Moderation
{
    public class HelpCategory
    {
        public int CategoryId { get; private set; }
        public string Caption { get; private set; }

        public HelpCategory(int id, string caption)
        {
            this.CategoryId = id;
            this.Caption = caption;
        }
    }
}
