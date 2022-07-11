using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Actions
{
    public class BotTeleport : WiredActionBase, IWired, IWiredEffect
    {
        public BotTeleport(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TELEPORT)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (string.IsNullOrWhiteSpace(this.StringParam) || this.Items.Count == 0)
            {
                return false;
            }

            RoomUser Bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(this.StringParam);
            if (Bot == null)
            {
                return false;
            }

            Item roomItem = this.Items[WibboEnvironment.GetRandomNumber(0, this.Items.Count - 1)];
            if (roomItem == null)
            {
                return false;
            }

            if (roomItem.Coordinate != Bot.Coordinate)
            {
                this.RoomInstance.GetGameMap().TeleportToItem(Bot, roomItem);
            }

            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
	            this.Delay = delay;
                
            this.StringParam = row["trigger_data"].ToString();

            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
                return;

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if (!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
