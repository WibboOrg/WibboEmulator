namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotTalk : WiredActionBase, IWired, IWiredEffect
{
    public BotTalk(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TALK) => this.IntParams.Add(0);

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

        var bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(name);
        if (bot == null)
        {
            return false;
        }

        var textMessage = message;
        if (user != null && user.Client != null)
        {
            textMessage = textMessage.Replace("#username#", user.GetUsername());
            textMessage = textMessage.Replace("#point#", user.WiredPoints.ToString());
            textMessage = textMessage.Replace("#roomname#", this.RoomInstance.Data.Name.ToString());
            textMessage = textMessage.Replace("#vote_yes#", this.RoomInstance.VotedYesCount.ToString());
            textMessage = textMessage.Replace("#vote_no#", this.RoomInstance.VotedNoCount.ToString());
            textMessage = textMessage.Replace("#wpcount#", user.Client.GetUser() != null ? user.Client.GetUser().WibboPoints.ToString() : "0");

            if (user.Roleplayer != null)
            {
                textMessage = textMessage.Replace("#money#", user.Roleplayer.Money.ToString());
            }
        }

        var isShout = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        bot.OnChat(textMessage, bot.IsPet ? 0 : 2, isShout);

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var isShout = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, isShout, null, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (bool.TryParse(row["all_user_triggerable"].ToString(), out var isShout))
        {
            this.IntParams.Add(isShout ? 1 : 0);
        }

        var data = row["trigger_data"].ToString();

        if (data == null)
        {
            return;
        }

        if (!data.Contains('\t'))
        {
            return;
        }

        this.StringParam = data;
    }
}
