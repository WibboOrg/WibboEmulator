namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class CollisionCase : WiredActionBase, IWiredEffect, IWired
{
    public CollisionCase(Item item, Room room) : base(item, room, (int)WiredActionType.COLLISION_CASE) => this.DefaultIntParams(new int[] { 0 });

    public override bool OnCycle(RoomUser user, Item item)
    {
        var isAllUser = this.GetIntParam(0) == 1;

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
        var isAllUser = this.GetIntParam(0);

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, isAllUser.ToString(), false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var isAllUser))
        {
            this.SetIntParam(0, isAllUser);
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
