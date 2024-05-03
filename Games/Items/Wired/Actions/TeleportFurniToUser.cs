namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class TeleportFurniToUser(Item item, Room room) : WiredActionBase(item, room, (int)WiredActionType.TELEPORT), IWired, IWiredCycleable, IWiredEffect
{
    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user == null)
        {
            return false;
        }

        var disableAnimation = this.Room.WiredHandler.DisableAnimate(this.Item.Coordinate);

        foreach (var roomItem in this.Items.ToList())
        {
            if (roomItem == null || roomItem.Coordinate == user.Coordinate)
            {
                continue;
            }

            this.Room.RoomItemHandling.PositionReset(roomItem, user.SetX, user.SetY, user.SetZ, disableAnimation);
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
