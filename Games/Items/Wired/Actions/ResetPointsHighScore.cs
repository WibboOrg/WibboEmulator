namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ResetPointsHighScore : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
{
    public ResetPointsHighScore(Item item, Room room) : base(item, room, (int)WiredActionType.CHASE)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (this.Items.Count == 0)
        {
            return false;
        }

        foreach (var hightScoreItem in this.Items)
        {
            if (hightScoreItem?.Data != null &&
                (hightScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE ||
                hightScoreItem.Data.InteractionType == InteractionType.HIGH_SCORE_POINTS))
            {
                hightScoreItem.Scores.Clear();
                hightScoreItem.UpdateState(false);
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
