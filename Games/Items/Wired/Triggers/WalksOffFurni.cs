namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms.Wired;

public class WalksOffFurni : WiredTriggerBase, IWired, IWiredCycleable
{
    public int DelayCycle => this.Delay;

    public WalksOffFurni(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_WALKS_OFF_FURNI) { }

    public bool OnCycle(RoomUser user, Item item)
    {
        if (user != null)
        {
            _ = this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, user, item);
        }

        return false;
    }

    private void OnUserWalksOffFurni(object obj, ItemTriggeredEventArgs args)
    {
        if (this.DelayCycle > 0)
        {
            this.RoomInstance.WiredHandler.RequestCycle(new WiredCycle(this, args.User, args.Item));
        }
        else
        {
            _ = this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, args.User, args.Item);
        }
    }

    public override void LoadItems(bool inDatabase = false)
    {
        base.LoadItems();

        if (this.Items != null)
        {
            foreach (var roomItem in this.Items.ToList())
            {
                roomItem.OnUserWalksOffFurni += this.OnUserWalksOffFurni;
            }
        }
    }

    public override void Dispose()
    {
        if (this.Items != null)
        {
            foreach (var roomItem in this.Items.ToList())
            {
                roomItem.OnUserWalksOffFurni -= this.OnUserWalksOffFurni;
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
