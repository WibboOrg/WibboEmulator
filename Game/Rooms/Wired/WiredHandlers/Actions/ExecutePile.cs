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

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleEffect(user, item);
            return false;
        }

        private void HandleEffect(RoomUser user, Item TriggerItem)
        {
            foreach (Item roomItem in this.Items)
            {
                if (roomItem.Coordinate != this.ItemInstance.Coordinate)
                {
                    this.RoomInstance.GetWiredHandler().ExecutePile(roomItem.Coordinate, user, TriggerItem);
                }
            }
        }

        public void Handle(RoomUser user, Item item)
        {
            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, item, this.DelayCycle));
            }
            else
            {
                this.HandleEffect(user, item);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, this.DelayCycle.ToString(), string.Empty, false, this.Items);
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
