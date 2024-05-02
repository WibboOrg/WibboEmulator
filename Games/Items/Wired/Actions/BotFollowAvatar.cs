namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class BotFollowAvatar : WiredActionBase, IWired, IWiredEffect
{
    public BotFollowAvatar(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_FOLLOW_AVATAR) => this.DefaultIntParams(0);

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (string.IsNullOrWhiteSpace(this.StringParam))
        {
            return false;
        }

        var bot = this.Room.RoomUserManager.GetBotOrPetByName(this.StringParam);
        if (bot == null)
        {
            return false;
        }

        if (user != null && !user.IsBot && user.Client != null)
        {
            var isFollow = this.GetIntParam(0) == 1;
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

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var isFollow = this.GetIntParam(0) == 1;

        WiredUtillity.SaveInDatabase(dbClient, this.Item.Id, string.Empty, this.StringParam, isFollow, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.SetIntParam(0, wiredAllUserTriggerable ? 1 : 0);

        this.StringParam = wiredTriggerData;
    }
}
