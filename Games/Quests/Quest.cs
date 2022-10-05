namespace WibboEmulator.Games.Quests;

public class Quest
{
    public int Id { get; private set; }
    public string Category { get; private set; }
    public int Number { get; private set; }
    public QuestType GoalType { get; private set; }
    public int GoalData { get; private set; }
    public string Name { get; private set; }
    public int Reward { get; private set; }
    public string DataBit { get; private set; }

    public string ActionName => QuestTypeUtillity.GetString(this.GoalType);

    public Quest(int id, string category, int number, QuestType goalType, int goalData, string name, int reward, string dataBit)
    {
        this.Id = id;
        this.Category = category;
        this.Number = number;
        this.GoalType = goalType;
        this.GoalData = goalData;
        this.Name = name;
        this.Reward = reward;
        this.DataBit = dataBit;
    }

    public bool IsCompleted(int userProgress)
    {
        if (this.GoalType != QuestType.EXPLORE_FIND_ITEM)
        {
            return userProgress >= (long)this.GoalData;
        }
        else
        {
            return userProgress >= 1;
        }
    }
}
