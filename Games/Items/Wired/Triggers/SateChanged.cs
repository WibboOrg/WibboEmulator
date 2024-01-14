namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class SateChanged : WiredTriggerBase, IWired
{
    public SateChanged(Item item, Room room) : base(item, room, (int)WiredTriggerType.TOGGLE_FURNI) { }

    private void OnTriggered(object sender, ItemTriggeredEventArgs e) => this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, e.User, e.Item);

    public override void LoadItems(bool inDatabase = false)
    {
        base.LoadItems();

        if (this.Items != null)
        {
            foreach (var roomItem in this.Items.ToList())
            {
                roomItem.ItemTrigger += this.OnTriggered;
            }
        }
    }

    public override void Dispose()
    {
        if (this.Items != null)
        {
            foreach (var roomItem in this.Items.ToList())
            {
                roomItem.ItemTrigger -= this.OnTriggered;
            }
        }

        base.Dispose();
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, string.Empty, false, this.Items);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.LoadStuffIds(wiredTriggersItem);
}
