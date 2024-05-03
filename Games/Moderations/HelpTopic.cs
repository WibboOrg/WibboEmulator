namespace WibboEmulator.Games.Moderations;

public class HelpTopic(int id, string caption, string body, int categoryId)
{
    public int TopicId { get; private set; } = id;
    public string Caption { get; private set; } = caption;
    public string Body { get; private set; } = body;
    public int CategoryId { get; private set; } = categoryId;
}
