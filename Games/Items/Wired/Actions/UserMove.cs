namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class UserMove : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
{
    public UserMove(Item item, Room room) : base(item, room, (int)WiredActionType.TELEPORT)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (this.Items.Count == 0 || user == null)
        {
            return false;
        }

        var roomItem = this.Items[0];
        if (roomItem == null)
        {
            return false;
        }

        if (roomItem.Coordinate != user.Coordinate)
        {
            user.IsWalking = true;
            user.GoalX = roomItem.X;
            user.GoalY = roomItem.Y;
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.Delay = delay;
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
