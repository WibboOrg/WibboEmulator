namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class CollisionRoom : WiredActionBase, IWiredEffect, IWired
{
    public CollisionRoom(Item item, Room room) : base(item, room, (int)WiredActionType.LEAVE_TEAM) => this.DefaultIntParams(0);

    public override bool OnCycle(RoomUser user, Item item)
    {
        foreach (var roomUser in this.Room.RoomUserManager.RoomUsers.ToList())
        {
            this.Room.WiredHandler.TriggerCollision(roomUser, item);
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.Delay = wiredDelay;
}
