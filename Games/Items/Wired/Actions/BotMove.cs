namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
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

        var bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(this.StringParam);
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

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.StringParam, false, this.Items, this.Delay);

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        this.StringParam = row["trigger_data"].ToString();

        var triggerItems = row["triggers_item"].ToString();

        if (triggerItems is null or "")
        {
            return;
        }

        foreach (var itemId in triggerItems.Split(';'))
        {
            if (!int.TryParse(itemId, out var id))
            {
                continue;
            }

            if (!this.StuffIds.Contains(id))
            {
                this.StuffIds.Add(id);
            }
        }
    }
}
