namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class BotReadchedAvatar : WiredTriggerBase, IWired
{
    public BotReadchedAvatar(Item item, Room room) : base(item, room, (int)WiredTriggerType.BOT_REACHED_AVATAR) => this.RoomInstance.WiredHandler.TrgBotCollision += this.OnCollision;

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

        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, args.User, null);
    }


    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.WiredHandler.TrgBotCollision -= this.OnCollision;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.ItemInstance.Id, string.Empty, this.StringParam);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.StringParam = wiredTriggerData;
}
