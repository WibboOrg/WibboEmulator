namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
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

            var usersToTrigger = new List<RoomUser>();

            foreach (var coord in roomItem.GetAffectedTiles)
            {
                usersToTrigger.AddRange(this.RoomInstance.RoomUserManager.GetUsersForSquare(coord.X, coord.Y));
            }

            foreach (var roomUser in usersToTrigger.Where(u => u != null).Take(isAllUser ? usersToTrigger.Count : 1))
            {
                this.RoomInstance.WiredHandler.TriggerCollision(roomUser, roomItem);
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var isAllUser = this.GetIntParam(0);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, isAllUser.ToString(), false, this.Items, this.Delay);
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
