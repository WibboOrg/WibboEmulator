namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class UserClick : WiredTriggerBase, IWired
{
    public UserClick(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_CLICK) => room.OnUserClick += this.OnUserClick;

    private void OnUserClick(object sender, UserTargetEventArgs e)
    {
        if (sender is null or not RoomUser)
        {
            return;
        }

        var user = (RoomUser)sender;
        var userTarget = e.UserTarget;

        if (user == null || user.IsBot || userTarget == null)
        {
            return;
        }

        var distance = (this.IntParams.Count > 0) ? this.IntParams[0] : 1;

        distance += 1;

        if (Math.Abs(userTarget.X - user.X) >= distance || Math.Abs(userTarget.Y - user.Y) >= distance)
        {
            return;
        }

        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, userTarget, null);
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.OnUserClick -= this.OnUserClick;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var distance = (this.IntParams.Count > 0) ? this.IntParams[0] : 1;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, distance.ToString(), false, null);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        var triggerData = row["trigger_data"].ToString();

        if (int.TryParse(triggerData, out var distance))
        {
            this.IntParams.Add(distance);
        }
    }
}
