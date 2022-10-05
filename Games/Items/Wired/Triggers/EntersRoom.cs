namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class EntersRoom : WiredTriggerBase, IWired
{
    private readonly RoomEventDelegate delegateFunction;

    public EntersRoom(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_ENTERS_ROOM)
    {
        this.delegateFunction = new RoomEventDelegate(this.OnUserEnter);
        this.RoomInstance.GetRoomUserManager().OnUserEnter += this.delegateFunction;
    }

    private void OnUserEnter(object sender, EventArgs e)
    {
        var user = (RoomUser)sender;
        if (user == null)
        {
            return;
        }

        if ((user.IsBot || string.IsNullOrEmpty(this.StringParam) || string.IsNullOrEmpty(this.StringParam) || !(user.GetUsername() == this.StringParam)) && !string.IsNullOrEmpty(this.StringParam))
        {
            return;
        }

        if (this.RoomInstance.GetWiredHandler() != null)
        {
            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, null);
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.GetRoomUserManager().OnUserEnter -= this.delegateFunction;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null);

    public void LoadFromDatabase(DataRow row) => this.StringParam = row["trigger_data"].ToString();
}
