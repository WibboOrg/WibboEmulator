namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotClothes : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
{
    public BotClothes(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_CHANGE_FIGURE)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (string.IsNullOrWhiteSpace(this.StringParam) || !this.StringParam.Contains('\t'))
        {
            return false;
        }

        var nameAndLook = this.StringParam.Split('\t');
        var nameBot = (nameAndLook.Length == 2) ? nameAndLook[0] : "";
        var look = (nameAndLook.Length == 2) ? nameAndLook[1] : "";

        if (nameBot == "" || look == "")
        {
            return false;
        }

        var bot = this.RoomInstance.RoomUserManager.GetBotOrPetByName(nameBot);
        if (bot == null)
        {
            return false;
        }

        bot.BotData.Look = look;

        this.RoomInstance.SendPacket(new UserChangeComposer(bot));

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        var triggerData = wiredTriggerData;

        if (string.IsNullOrWhiteSpace(triggerData) || !triggerData.Contains('\t'))
        {
            return;
        }

        this.StringParam = triggerData;
    }
}
