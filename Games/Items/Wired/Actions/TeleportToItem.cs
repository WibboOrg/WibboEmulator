namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Utilities;

public class TeleportToItem : WiredActionBase, IWired, IWiredCycleable, IWiredEffect
{
    public new bool IsTeleport => true;

    public TeleportToItem(Item item, Room room) : base(item, room, (int)WiredActionType.TELEPORT) => this.Delay = 1;

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user == null)
        {
            return false;
        }

        if (!user.PendingTeleport)
        {
            user.PendingTeleport = true;

            user.ApplyEffect(4, true);
            user.Freeze = true;
            return true;
        }

        if (this.Items.Count > 1)
        {
            var roomItem = this.Items.GetRandomElement();
            if (roomItem == null)
            {
                return false;
            }

            if (roomItem.Coordinate != user.Coordinate)
            {
                GameMap.TeleportToItem(user, roomItem);
            }
        }
        else if (this.Items.Count == 1)
        {
            GameMap.TeleportToItem(user, Enumerable.FirstOrDefault(this.Items));
        }

        user.PendingTeleport = false;
        user.ApplyEffect(user.CurrentEffect, true);
        if (user.FreezeEndCounter <= 0)
        {
            user.Freeze = false;
        }

        return false;
    }

    public override void Handle(RoomUser user, Item item)
    {
        if (this.Items.Count == 0 || user == null)
        {
            return;
        }

        if (user.PendingTeleport)
        {
            return;
        }

        if (this.Delay <= 1)
        {
            user.PendingTeleport = true;

            user.ApplyEffect(4, true);
            user.Freeze = true;
            if (user.ContainStatus("mv"))
            {
                user.RemoveStatus("mv");
                user.UpdateNeeded = true;
            }
        }

        base.Handle(user, item);
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.Delay = delay + 1;
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
