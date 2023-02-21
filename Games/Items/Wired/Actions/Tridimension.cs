namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class Tridimension : WiredActionBase, IWiredEffect, IWired
{
    public Tridimension(Item item, Room room) : base(item, room, (int)WiredActionType.TRI_DIMENSION) => this.StringParam = "0;0;0.0";

    public override bool OnCycle(RoomUser user, Item item)
    {
        foreach (var roomItem in this.Items.ToList())
        {
            this.HandleMovement(roomItem);
        }

        return false;
    }

    private void HandleMovement(Item item)
    {
        if (this.RoomInstance.RoomItemHandling.GetItem(item.Id) == null)
        {
            return;
        }

        var parts = this.StringParam.Split(';');

        var x = 0;
        var y = 0;
        var z = 0.0;

        if (parts.Length == 3)
        {
            _ = int.TryParse(parts[0], out x);
            _ = int.TryParse(parts[1], out y);
            _ = double.TryParse(parts[2], out z);
        }

        var newX = item.X + x;
        var newY = item.Y + y;
        double newZ = item.Z + z;
        if (newX != item.X || newY != item.Y || newZ != item.Z)
        {
            this.RoomInstance.RoomItemHandling.PositionReset(item, newX, newY, newZ);
        }

        return;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var movement = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var rotation = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        var rotAndMove = rotation + ";" + movement;
        WiredUtillity.SaveTriggerItem(dbClient, this.Id, rotAndMove, string.Empty, false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data"].ToString(), out delay))
        {
            this.Delay = delay;
        }

        var triggerData2 = row["trigger_data_2"].ToString();
        if (triggerData2 != null && triggerData2.Contains(';'))
        {
            if (int.TryParse(triggerData2.Split(';')[1], out var movement))
            {
                this.IntParams.Add(movement);
            }

            if (int.TryParse(triggerData2.Split(';')[0], out var rotationint))
            {
                this.IntParams.Add(rotationint);
            }
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
