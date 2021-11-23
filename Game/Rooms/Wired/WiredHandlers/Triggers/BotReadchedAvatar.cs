using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class BotReadchedAvatar : WiredTriggerBase, IWired
    {
        private readonly BotCollisionDelegate delegateFunction;

        public BotReadchedAvatar(Item item, Room room) : base(item, room, (int)WiredTriggerType.BOT_REACHED_AVATAR)
        {
            this.delegateFunction = new BotCollisionDelegate(this.Collision);
            this.RoomInstance.GetWiredHandler().TrgBotCollision += this.delegateFunction;
        }

        private void Collision(RoomUser user, string botName)
        {
            if (user == null || user.IsBot)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.StringParam) && this.StringParam != botName)
            {
                return;
            }

            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, null);
        }


        public override void Dispose()
        {
            base.Dispose();

            this.RoomInstance.GetWiredHandler().TrgBotCollision -= this.delegateFunction;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.StringParam, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.StringParam = row["trigger_data"].ToString();
        }
    }
}
