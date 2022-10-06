namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class UserCollision : WiredTriggerBase, IWired
{
    private readonly RoomEventDelegate _delegateFunction;

    public UserCollision(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION)
    {
        this._delegateFunction = new RoomEventDelegate(this.UserCollisionDelegate);
        this.RoomInstance.GetWiredHandler().GetRoom().OnUserCls += this._delegateFunction;
    }

    private void UserCollisionDelegate(object sender, EventArgs e)
    {
        var user = (RoomUser)sender;
        if (user == null)
        {
            return;
        }

        this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, null);
    }
    public override void Dispose()
    {
        this.RoomInstance.GetWiredHandler().GetRoom().OnUserCls -= this._delegateFunction;

        base.Dispose();
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, null);

    public void LoadFromDatabase(DataRow row)
    {
    }
}
