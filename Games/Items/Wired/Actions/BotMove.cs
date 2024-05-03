namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotMove : WiredActionBase, IWired, IWiredEffect
{
    public BotMove(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_MOVE) => this.FurniLimit = 1;

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (this.StringParam == "" || this.Items.Count == 0)
        {
            return false;
        }

        var bot = this.Room.RoomUserManager.GetBotOrPetByName(this.StringParam);
        if (bot == null)
        {
            return false;
        }

        var itemTeleport = this.Items[0];
        if (itemTeleport == null)
        {
            return false;
        }

        if (itemTeleport.Coordinate != bot.Coordinate)
        {
            bot.MoveTo(itemTeleport.X, itemTeleport.Y, true);
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Item.Id, string.Empty, this.StringParam, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
