namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class UserCommand : WiredTriggerBase, IWired
{
    public UserCommand(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION) => room.OnTrigger += this.OnUserSays;

    private void OnUserSays(object sender, EventArgs e)
    {
        var user = (RoomUser)sender;
        if (user == null || user.IsBot)
        {
            return;
        }

        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, user, null);
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.OnTrigger -= this.OnUserSays;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {

    }

    public void LoadFromDatabase(DataRow row)
    {
    }
}
