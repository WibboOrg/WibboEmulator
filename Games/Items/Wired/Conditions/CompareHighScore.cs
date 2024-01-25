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

        var highScoreItem = this.Items.FirstOrDefault();

        if (highScoreItem != null && highScoreItem.Data != null &&
            (highScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE || highScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE_POINTS))
        {
            var highScoreOperator = (HighScoreOperatorCondition)this.GetIntParam(0);

            _ = int.TryParse(this.StringParam, out var valueInt);
            if (this.StringParam == "#point#")
            {
                valueInt = user.WiredPoints;
            }

            var inHighScore = highScoreItem.Scores.TryGetValue(user.GetUsername(), out var score);

            switch (highScoreOperator)
            {
                case HighScoreOperatorCondition.Equal:
                    return score == valueInt;
                case HighScoreOperatorCondition.NotEqual:
                    return score != valueInt;
                case HighScoreOperatorCondition.LessThanOrEqual:
                    return score <= valueInt;
                case HighScoreOperatorCondition.LessThan:
                    return score < valueInt;
                case HighScoreOperatorCondition.GreaterThanOrEqual:
                    return score >= valueInt;
                case HighScoreOperatorCondition.GreaterThan:
                    return score > valueInt;
                case HighScoreOperatorCondition.Modulo:
                    return valueInt == 0 || score % valueInt == 0;
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
        WiredUtillity.SaveInDatabase(dbClient, this.Id, highScoreOperator.ToString(), this.StringParam, false, this.Items);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;

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
    Modulo,
    InHighScore,
    InNotHighScore
}