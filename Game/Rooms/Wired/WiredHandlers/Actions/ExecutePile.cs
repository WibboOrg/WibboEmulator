using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class ExecutePile : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
    {
        public ExecutePile(Item item, Room room) : base(item, room, (int)WiredActionType.TOGGLE_FURNI_STATE)
        {
            
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            foreach (Item roomItem in this.Items)
            {
                if (roomItem.Coordinate != this.ItemInstance.Coordinate)
                {
                    this.RoomInstance.GetWiredHandler().ExecutePile(roomItem.Coordinate, user, item);
                }
            }

            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, this.DelayCycle.ToString(), string.Empty, false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data_2"].ToString(), out int delay))
                this.Delay = delay;

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
