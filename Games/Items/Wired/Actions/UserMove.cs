using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Actions
{
    public class UserMove : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
    {
        public UserMove(Item item, Room room) : base(item, room, (int)WiredActionType.TELEPORT)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (this.Items.Count == 0 || user == null)
            {
                return false;
            }

            Item roomItem = this.Items[0];
            if (roomItem == null)
            {
                return false;
            }

            if (roomItem.Coordinate != user.Coordinate)
            {
                user.IsWalking = true;
                user.GoalX = roomItem.X;
                user.GoalY = roomItem.Y;
            }

            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            int delay;
            if (int.TryParse(row["delay"].ToString(), out delay))
	            this.Delay = delay;

            if (int.TryParse(row["trigger_data"].ToString(), out delay))
                this.Delay = delay;

            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
            {
                return;
            }

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
