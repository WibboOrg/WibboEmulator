namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ShowMessage : WiredActionBase, IWired, IWiredEffect
{
    public ShowMessage(Item item, Room room) : base(item, room, (int)WiredActionType.CHAT)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (this.StringParam == "")
        {
            return false;
        }

        if (user != null && !user.IsBot && user.Client != null)
        {
            var textMessage = this.StringParam;
            WiredUtillity.ParseMessage(user, this.RoomInstance, ref textMessage);

            user.SendWhisperChat(textMessage);
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData2, out var delay))
        {
            this.Delay = delay;
        }

        this.StringParam = wiredTriggerData;
    }
}
