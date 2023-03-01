namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class UserExit : WiredTriggerBase, IWired
{
    public UserExit(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION) => this.RoomInstance.RoomUserManager.OnUserExit += this.OnUserExit;

    private void OnUserExit(object sender, EventArgs e)
    {
        if (sender is null or not RoomUser)
        {
            return;
        }

        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, (RoomUser)sender, null);
    }
    public override void Dispose()
    {
        this.RoomInstance.RoomUserManager.OnUserExit -= this.OnUserExit;

        base.Dispose();
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, null);

    public void LoadFromDatabase(DataRow row)
    {
    }
}
