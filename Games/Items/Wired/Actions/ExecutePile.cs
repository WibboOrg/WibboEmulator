using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Items.Wired.Interfaces;
using System.Data;
using System.Drawing;

namespace WibboEmulator.Games.Items.Wired.Actions
{
    public class ExecutePile : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
    {
        public ExecutePile(Item item, Room room) : base(item, room, (int)WiredActionType.CALL_ANOTHER_STACK)
        {
            
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            foreach (Item roomItem in this.Items.ToList())
            {
                foreach (Point Coord in roomItem.GetCoords)
                {
                    if (Coord != this.ItemInstance.Coordinate)
                    {
                        this.RoomInstance.GetWiredHandler().ExecutePile(Coord, user, item);
                    }
                }
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

            if (int.TryParse(row["trigger_data_2"].ToString(), out delay))
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
