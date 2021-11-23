using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;
using System.Drawing;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class ItemUserCollision : WiredActionBase, IWiredEffect, IWired
    {
        public ItemUserCollision(Item item, Room room) : base(item, room, (int)WiredActionType.FLEE)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            foreach (Item roomItem in this.Items.ToArray())
            {
                this.HandleMovement(roomItem);
            }

            return false;
        }

        private void HandleMovement(Item item)
        {
            if (this.RoomInstance.GetRoomItemHandler().GetItem(item.Id) == null)
            {
                return;
            }

            foreach (Point Coord in item.GetCoords)
            {
                RoomUser roomUser = this.RoomInstance.GetRoomUserManager().GetUserForSquare(Coord.X, Coord.Y);
                if (roomUser != null)
                {
                    this.RoomInstance.GetWiredHandler().TriggerCollision(roomUser, item);
                    return;
                }
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["delay"].ToString(), out int delay))
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
