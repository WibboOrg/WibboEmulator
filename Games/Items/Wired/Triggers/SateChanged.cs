namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms.Wired;

public class SateChanged : WiredTriggerBase, IWired, IWiredCycleable
{
    public int DelayCycle => this.Delay;

    public SateChanged(Item item, Room room) : base(item, room, (int)WiredTriggerType.TOGGLE_FURNI) { }

    public bool OnCycle(RoomUser user, Item item)
    {
        if (user != null)
        {
            this.OnTriggered(user, item);
        }

        return false;
    }

    private void OnTriggered(object sender, ItemTriggeredEventArgs e)
    {
        if (this.Delay > 0)
        {
            this.RoomInstance.WiredHandler.RequestCycle(new WiredCycle(this, e.User, e.Item));
        }
        else
        {
            this.OnTriggered(e.User, e.Item);
        }
    }

    private void OnTriggered(RoomUser user, Item item) => this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, user, item);

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

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.DelayCycle.ToString(), false, this.Items);

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["trigger_data"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        var triggerItems = row["triggers_item"].ToString();

        if (triggerItems is null or "")
        {
            return;
        }

        foreach (var itemId in triggerItems.Split(';'))
        {
            if (!int.TryParse(itemId, out var id))
            {
                continue;
            }

            if (!this.StuffIds.Contains(id))
            {
                this.StuffIds.Add(id);
            }
        }
    }
}
