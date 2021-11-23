using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class BotMove : WiredActionBase, IWired, IWiredEffect
    {
        public BotMove(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_MOVE)
        {
            this.FurniLimit = 1;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.StringParam == "" || this.Items.Count == 0)
            {
                return;
            }

            RoomUser bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(this.StringParam);
            if (bot == null)
            {
                return;
            }

            Item item = this.Items[0];
            if (item == null)
            {
                return;
            }

            if (item.Coordinate != bot.Coordinate)
            {
                bot.MoveTo(item.GetX, item.GetY, true);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.StringParam, false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.StringParam = row["trigger_data"].ToString();

            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
                return;

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if(!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
