namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class CollisionCase : WiredActionBase, IWiredEffect, IWired
{
    public CollisionCase(Item item, Room room) : base(item, room, (int)WiredActionType.COLLISION_CASE)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        var isAllUser = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        foreach (var roomItem in this.Items.ToList())
        {
            if (this.RoomInstance.RoomItemHandling.GetItem(roomItem.Id) == null)
            {
                continue;
            }

            if (isAllUser)
            {
                var roomUsers = this.RoomInstance.RoomUserManager.GetUsersForSquare(roomItem.X, roomItem.Y);
                if (roomUsers.Count != 0)
                {
                    foreach (var roomUser in roomUsers)
                    {
                        this.RoomInstance.WiredHandler.TriggerCollision(roomUser, roomItem);
                    }
                }
            }
            else
            {
                var roomUser = this.RoomInstance.RoomUserManager.GetUserForSquare(roomItem.X, roomItem.Y);
                if (roomUser != null)
                {
                    this.RoomInstance.WiredHandler.TriggerCollision(roomUser, roomItem);
                }
            }
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var isAllUser = this.IntParams.Count > 0 ? this.IntParams[0] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, isAllUser.ToString(), false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.IntParams.Clear();

        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var isAllUser))
        {
            this.IntParams.Add(isAllUser);
        }

        var triggerItems = wiredTriggersItem;

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
