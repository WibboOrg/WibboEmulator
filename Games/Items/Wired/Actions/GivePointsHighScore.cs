namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class GivePointsHighScore : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
{
    public GivePointsHighScore(Item item, Room room) : base(item, room, (int)WiredActionType.GIVE_POINTS_HIGHSCORE) => this.DefaultIntParams(new int[] { 0 });

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user == null || user.IsBot || user.Client == null)
        {
            return false;
        }

        _ = int.TryParse(this.StringParam, out var valueInt);
        if (this.StringParam == "#point#")
        {
            valueInt = user.WiredPoints;
        }

        var highScoreOperator = (HighScoreOperatorAction)this.GetIntParam(0);

        foreach (var highScoreItem in this.Items)
        {
            if (highScoreItem?.Data != null &&
                (highScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE ||
                highScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE_POINTS))
            {
                var scores = highScoreItem.Scores;

                if (scores.TryGetValue(user.GetUsername(), out var score))
                {
                    switch (highScoreOperator)
                    {
                        case HighScoreOperatorAction.Addition:
                            scores[user.GetUsername()] = score <= int.MaxValue - valueInt ? score + valueInt : valueInt;
                            break;
                        case HighScoreOperatorAction.Subtraction:
                            scores[user.GetUsername()] = score >= int.MinValue + valueInt ? score - valueInt : valueInt;
                            break;
                        case HighScoreOperatorAction.Multiplication:
                            scores[user.GetUsername()] = unchecked(score * valueInt);
                            break;
                        case HighScoreOperatorAction.Division:
                            scores[user.GetUsername()] = valueInt != 0 ? (int)Math.Round((double)score / valueInt) : 0;
                            break;
                        case HighScoreOperatorAction.Modulo:
                            scores[user.GetUsername()] = valueInt != 0 ? score % valueInt : 0;
                            break;
                        case HighScoreOperatorAction.Remplace:
                            scores[user.GetUsername()] = valueInt;
                            break;
                        case HighScoreOperatorAction.Delete:
                            _ = scores.Remove(user.GetUsername());
                            break;
                    }
                }
                else if (highScoreOperator != HighScoreOperatorAction.Delete)
                {
                    scores.Add(user.GetUsername(), valueInt);
                }

                highScoreItem.UpdateState(false);
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var highScoreOperator = this.GetIntParam(0);
        WiredUtillity.SaveInDatabase(dbClient, this.Id, highScoreOperator.ToString(), this.StringParam, false, this.Items, this.Delay);
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

public enum HighScoreOperatorAction
{
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Modulo,
    Remplace,
    Delete
}