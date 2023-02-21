namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
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
        textMessage = textMessage.Replace("#roomname#", this.RoomInstance.RoomData.Name.ToString());
        textMessage = textMessage.Replace("#vote_yes#", this.RoomInstance.VotedYesCount.ToString());
        textMessage = textMessage.Replace("#vote_no#", this.RoomInstance.VotedNoCount.ToString());

        foreach (var userTarget in this.RoomInstance.RoomUserManager.GetUserList().ToList())
        {
            userTarget?.SendWhisperChat(textMessage);
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data_2"].ToString(), out delay))
        {
            this.Delay = delay;
        }

        this.StringParam = row["trigger_data"].ToString();
    }
}
