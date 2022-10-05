namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
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

        if (user != null && !user.IsBot && user.GetClient() != null)
        {
            var textMessage = this.StringParam;
            textMessage = textMessage.Replace("#username#", user.GetUsername());
            textMessage = textMessage.Replace("#point#", user.WiredPoints.ToString());
            textMessage = textMessage.Replace("#roomname#", this.RoomInstance.GetWiredHandler().GetRoom().RoomData.Name.ToString());
            textMessage = textMessage.Replace("#vote_yes#", this.RoomInstance.GetWiredHandler().GetRoom().VotedYesCount.ToString());
            textMessage = textMessage.Replace("#vote_no#", this.RoomInstance.GetWiredHandler().GetRoom().VotedNoCount.ToString());
            textMessage = textMessage.Replace("#wpcount#", user.GetClient().GetUser() != null ? user.GetClient().GetUser().WibboPoints.ToString() : "0");

            if (user.Roleplayer != null)
            {
                textMessage = textMessage.Replace("#money#", user.Roleplayer.Money.ToString());
            }

            user.SendWhisperChat(textMessage);
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
