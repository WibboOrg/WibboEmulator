namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class TeleportFurniToUser : WiredActionBase, IWired, IWiredCycleable, IWiredEffect
{
    public TeleportFurniToUser(Item item, Room room) : base(item, room, (int)WiredActionType.TELEPORT)
    { }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user == null)
        {
            return false;
        }

        var disableAnimation = this.RoomInstance.WiredHandler.DisableAnimate(this.ItemInstance.Coordinate);

        if (this.Items.Count > 1)
        {
            var roomItem = this.Items[WibboEnvironment.GetRandomNumber(0, this.Items.Count - 1)];
            if (roomItem == null)
            {
                return false;
            }

            if (roomItem.Coordinate != user.Coordinate)
            {
                this.RoomInstance.RoomItemHandling.PositionReset(roomItem, user.SetX, user.SetY, user.SetZ, disableAnimation);
            }
        }
        else if (this.Items.Count == 1)
        {
            this.RoomInstance.RoomItemHandling.PositionReset(Enumerable.FirstOrDefault(this.Items), user.SetX, user.SetY, user.SetZ, disableAnimation);
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data"].ToString(), out delay))
        {
            this.Delay = delay + 1;
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
