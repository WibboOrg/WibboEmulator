namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

public class BotTeleport : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
{
    public new bool IsTeleport => true;

    public BotTeleport(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TELEPORT)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (string.IsNullOrWhiteSpace(this.StringParam) || this.Items.Count == 0)
        {
            return false;
        }

        var bot = this.RoomInstance.RoomUserManager.GetBotOrPetByName(this.StringParam);
        if (bot == null)
        {
            return false;
        }

        if (!bot.PendingTeleport && this.Delay > 0)
        {
            bot.PendingTeleport = true;

            bot.ApplyEffect(4, true);
            bot.Freeze = true;
            return true;
        }

        var roomItem = this.Items[WibboEnvironment.GetRandomNumber(0, this.Items.Count - 1)];
        if (roomItem == null)
        {
            return false;
        }

        if (roomItem.Coordinate != bot.Coordinate)
        {
            GameMap.TeleportToItem(bot, roomItem);
        }

        bot.PendingTeleport = false;
        bot.ApplyEffect(bot.CurrentEffect, true);
        if (bot.FreezeEndCounter <= 0)
        {
            bot.Freeze = false;
        }

        return false;
    }

    public override void Handle(RoomUser user, Item item)
    {
        if (string.IsNullOrWhiteSpace(this.StringParam) || this.Items.Count == 0)
        {
            return;
        }

        var bot = this.RoomInstance.RoomUserManager.GetBotOrPetByName(this.StringParam);
        if (bot == null)
        {
            return;
        }

        if (!bot.PendingTeleport && this.Delay <= 1)
        {
            bot.PendingTeleport = true;

            bot.ApplyEffect(4, true);
            bot.Freeze = true;
            if (bot.ContainStatus("mv"))
            {
                bot.RemoveStatus("mv");
                bot.UpdateNeeded = true;
            }
        }

        base.Handle(user, item);
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;

        var triggerItems = wiredTriggersItem;

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
