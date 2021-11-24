using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class BotFollowAvatar : WiredActionBase, IWired, IWiredEffect
    {
        public BotFollowAvatar(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_FOLLOW_AVATAR)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (string.IsNullOrWhiteSpace(this.StringParam))
            {
                return false;
            }

            RoomUser bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(this.StringParam);
            if (bot == null)
            {
                return false;
            }

            if (user != null && !user.IsBot && user.GetClient() != null)
            {
                bool isFollow = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1);
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
            bool isFollow = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1);

            WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.StringParam, isFollow, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;

            if (bool.TryParse(row["all_user_triggerable"].ToString(), out bool isFollow))
                this.IntParams.Add(isFollow ? 1 : 0);

            this.StringParam = row["trigger_data"].ToString();
        }
    }
}
