namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotFollowAvatar : WiredActionBase, IWired, IWiredEffect
{
    public BotFollowAvatar(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_FOLLOW_AVATAR) => this.IntParams.Add(0);

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (string.IsNullOrWhiteSpace(this.StringParam))
        {
            return false;
        }

        var bot = this.RoomInstance.RoomUserManager.GetBotOrPetByName(this.StringParam);
        if (bot == null)
        {
            return false;
        }

        if (user != null && !user.IsBot && user.Client != null)
        {
            var isFollow = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;
            if (isFollow)
            {
                if (bot.BotData.FollowUser != user.VirtualId)
                {
                    bot.BotData.FollowUser = user.VirtualId;
                }
            }
            else
            {
                bot.BotData.FollowUser = 0;
            }
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var isFollow = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.StringParam, isFollow, null, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (bool.TryParse(row["all_user_triggerable"].ToString(), out var isFollow))
        {
            this.IntParams.Add(isFollow ? 1 : 0);
        }

        this.StringParam = row["trigger_data"].ToString();
    }
}
