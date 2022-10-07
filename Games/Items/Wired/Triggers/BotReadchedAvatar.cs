namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities.Events;

public class BotReadchedAvatar : WiredTriggerBase, IWired
{
    public BotReadchedAvatar(Item item, Room room) : base(item, room, (int)WiredTriggerType.BOT_REACHED_AVATAR) => this.RoomInstance.GetWiredHandler().TrgBotCollision += this.OnCollision;

    private void OnCollision(object obj, ItemTriggeredEventArgs args)
    {
        if (args.User == null || args.User.IsBot)
        {
            return;
        }

        if (!string.IsNullOrEmpty(this.StringParam) && this.StringParam != args.Value)
        {
            return;
        }

        this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, args.User, null);
    }


    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.GetWiredHandler().TrgBotCollision -= this.OnCollision;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.StringParam, false, null);

    public void LoadFromDatabase(DataRow row) => this.StringParam = row["trigger_data"].ToString();
}
