namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotTalk : WiredActionBase, IWired, IWiredEffect
{
    public BotTalk(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TALK) => this.DefaultIntParams(new int[] { 0 });

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (!this.StringParam.Contains('\t'))
        {
            return false;
        }

        var messageAndName = this.StringParam.Split('\t');
        var name = (messageAndName.Length == 2) ? messageAndName[0] : "";
        var message = (messageAndName.Length == 2) ? messageAndName[1] : "";

        if (name == "" || message == "")
        {
            return false;
        }

        var bot = this.RoomInstance.RoomUserManager.GetBotOrPetByName(name);
        if (bot == null)
        {
            return false;
        }

        WiredUtillity.ParseMessage(user, this.RoomInstance, ref message);

        var isShout = this.GetIntParam(0) == 1;

        bot.OnChat(message, bot.IsPet ? 0 : 2, isShout);

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var isShout = this.GetIntParam(0) == 1;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, isShout, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.SetIntParam(0, wiredAllUserTriggerable ? 1 : 0);

        this.StringParam = wiredTriggerData;
    }
}
