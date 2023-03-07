namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotGiveHanditem : WiredActionBase, IWired, IWiredEffect
{
    public BotGiveHanditem(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_GIVE_HAND_ITEM) => this.IntParams.Add(0);

    public override bool OnCycle(RoomUser user, Item item) => false;

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var handItemId = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, handItemId.ToString(), this.StringParam, false, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.IntParams.Clear();

        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;

        if (int.TryParse(wiredTriggerData2, out var handItemId))
        {
            this.IntParams.Add(handItemId);
        }
    }
}
