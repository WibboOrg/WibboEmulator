namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class UserCommand : WiredTriggerBase, IWired
{
    private readonly RoomEventDelegate _delegateFunction;

    public UserCommand(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION)
    {
        this._delegateFunction = new RoomEventDelegate(this.RoomUserManager_OnUserSays);
        room.OnTrigger += this._delegateFunction;
    }

    private void RoomUserManager_OnUserSays(object sender, EventArgs e)
    {
        var user = (RoomUser)sender;
        if (user == null || user.IsBot)
        {
            return;
        }

        this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, null);
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.GetWiredHandler().GetRoom().OnTrigger -= this._delegateFunction;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {

    }

    public void LoadFromDatabase(DataRow row)
    {
    }
}
