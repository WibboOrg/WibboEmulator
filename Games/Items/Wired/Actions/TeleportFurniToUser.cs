namespace WibboEmulator.Games.Items.Wired.Actions;
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

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
