namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class SateChanged(Item item, Room room) : WiredTriggerBase(item, room, (int)WiredTriggerType.TOGGLE_FURNI), IWired
{
    private void OnTriggered(object sender, ItemTriggeredEventArgs e) => this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, e.User, e.Item);

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

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Item.Id, string.Empty, string.Empty, false, this.Items);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.LoadStuffIds(wiredTriggersItem);
}
