namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ResetPointsHighScore(Item item, Room room) : WiredActionBase(item, room, (int)WiredActionType.CHASE), IWired, IWiredEffect, IWiredCycleable
{
    public override bool OnCycle(RoomUser user, Item item)
    {
        if (this.Items.Count == 0)
        {
            return false;
        }

        foreach (var highScoreItem in this.Items)
        {
            if (highScoreItem?.Data != null &&
                (highScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE ||
                highScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE_POINTS))
            {
                highScoreItem.Scores.Clear();
                highScoreItem.UpdateState(false);
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
