namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ShowMessageRoom : WiredActionBase, IWired, IWiredEffect
{
    public ShowMessageRoom(Item item, Room room) : base(item, room, (int)WiredActionType.CHAT)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (this.StringParam == "")
        {
            return false;
        }

        var textMessage = this.StringParam;
        WiredUtillity.ParseMessage(user, this.RoomInstance, ref textMessage);

        foreach (var userTarget in this.RoomInstance.RoomUserManager.GetUserList().ToList())
        {
            userTarget?.SendWhisperChat(textMessage);
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;
    }
}
