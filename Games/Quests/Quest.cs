namespace WibboEmulator.Games.Quests;

public class Quest(int id, string category, int number, QuestType goalType, int goalData, string name, int reward, string dataBit)
{
    public int Id { get; private set; } = id;
    public string Category { get; private set; } = category;
    public int Number { get; private set; } = number;
    public QuestType GoalType { get; private set; } = goalType;
    public int GoalData { get; private set; } = goalData;
    public string Name { get; private set; } = name;
    public int Reward { get; private set; } = reward;
    public string DataBit { get; private set; } = dataBit;

    public string ActionName => QuestTypeUtillity.GetString(this.GoalType);

    public bool IsCompleted(int userProgress)
    {
        if (this.GoalType != QuestType.ExploreFindItem)
        {
            return userProgress >= (long)this.GoalData;
        }
        else
        {
            return userProgress >= 1;
        }
    }
}
