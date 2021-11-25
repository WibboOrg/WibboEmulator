namespace Butterfly.Game.Moderation
{
    public class HelpTopic
    {
        public int TopicId { get; private set; }
        public string Caption { get; private set; }
        public string Body { get; private set; }
        public int CategoryId { get; private set; }

        public HelpTopic(int id, string caption, string body, int categoryId)
        {
            this.TopicId = id;
            this.Caption = caption;
            this.Body = body;
            this.CategoryId = categoryId;
        }
    }
}
