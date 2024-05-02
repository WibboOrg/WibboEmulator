namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class UserCollision : WiredTriggerBase, IWired
{
    public UserCollision(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION) => this.Room.OnUserCls += this.OnUserCollision;

    private void OnUserCollision(object sender, EventArgs e)
    {
        if (sender is null or not RoomUser)
        {
            return;
        }

        this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, (RoomUser)sender, null);
    }
    public override void Dispose()
    {
        this.Room.OnUserCls -= this.OnUserCollision;

        base.Dispose();
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
    }
}
