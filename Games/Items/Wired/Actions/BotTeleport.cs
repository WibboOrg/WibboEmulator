namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotTeleport : WiredActionBase, IWired, IWiredEffect
{
    public BotTeleport(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TELEPORT)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (string.IsNullOrWhiteSpace(this.StringParam) || this.Items.Count == 0)
        {
            return false;
        }

        var bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(this.StringParam);
        if (bot == null)
        {
            return false;
        }

        var roomItem = this.Items[WibboEnvironment.GetRandomNumber(0, this.Items.Count - 1)];
        if (roomItem == null)
        {
            return false;
        }

        if (roomItem.Coordinate != bot.Coordinate)
        {
            this.RoomInstance.GetGameMap().TeleportToItem(bot, roomItem);
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, this.Items, this.Delay);

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
