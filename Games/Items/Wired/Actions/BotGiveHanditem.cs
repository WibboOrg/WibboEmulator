namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotGiveHanditem : WiredActionBase, IWired, IWiredEffect
{
    public BotGiveHanditem(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_GIVE_HAND_ITEM) => this.DefaultIntParams(new int[] { 0 });

    public override bool OnCycle(RoomUser user, Item item) => false;

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var handItemId = this.GetIntParam(0);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, handItemId.ToString(), this.StringParam, false, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;

        if (int.TryParse(wiredTriggerData2, out var handItemId))
        {
            this.SetIntParam(0, handItemId);
        }
    }
}
