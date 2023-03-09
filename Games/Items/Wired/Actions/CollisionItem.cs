namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class CollisionItem : WiredActionBase, IWiredEffect, IWired
{
    public CollisionItem(Item item, Room room) : base(item, room, (int)WiredActionType.CHASE)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        foreach (var roomItem in this.Items.ToList())
        {
            if (this.RoomInstance.RoomItemHandling.GetItem(roomItem.Id) == null)
            {
                continue;
            }

            this.RoomInstance.WiredHandler.TriggerCollision(null, roomItem);
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
