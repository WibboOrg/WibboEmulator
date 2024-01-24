namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class CompareHighScore : WiredConditionBase, IWiredCondition, IWired
{
    public CompareHighScore(Item item, Room room) : base(item, room, (int)WiredConditionType.ACTOR_COMPARE_HIGHSCORE)
    {
        this.DefaultIntParams(new int[] { 0 });
        this.FurniLimit = 1;
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (user == null || user.IsBot || user.Client == null)
        {
            return false;
        }

        var hightScoreItem = this.Items.FirstOrDefault();

        if (hightScoreItem != null && hightScoreItem.Data != null &&
            (hightScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE || hightScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE_POINTS))
        {
            var highScoreOperator = (HighScoreOperatorCondition)this.GetIntParam(0);

            if (!int.TryParse(this.StringParam, out var valueInt))
            {
                return false;
            }

            var inHighScore = hightScoreItem.Scores.TryGetValue(user.GetUsername(), out var score);

            switch (highScoreOperator)
            {
                case HighScoreOperatorCondition.Equal:
                    return valueInt == score;
                case HighScoreOperatorCondition.NotEqual:
                    return valueInt != score;
                case HighScoreOperatorCondition.LessThanOrEqual:
                    return valueInt <= score;
                case HighScoreOperatorCondition.LessThan:
                    return valueInt < score;
                case HighScoreOperatorCondition.GreaterThanOrEqual:
                    return valueInt >= score;
                case HighScoreOperatorCondition.GreaterThan:
                    return valueInt > score;
                case HighScoreOperatorCondition.InHighScore:
                    return inHighScore;
                case HighScoreOperatorCondition.InNotHighScore:
                    return !inHighScore;
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var highScoreOperator = this.GetIntParam(0);
        WiredUtillity.SaveInDatabase(dbClient, this.Id, highScoreOperator.ToString(), this.StringParam, false, null);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData2, out var highScoreOperator))
        {
            this.IntParams.Add(highScoreOperator);
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}

public enum HighScoreOperatorCondition
{
    Equal,
    NotEqual,
    LessThanOrEqual,
    LessThan,
    GreaterThanOrEqual,
    GreaterThan,
    InHighScore,
    InNotHighScore
}