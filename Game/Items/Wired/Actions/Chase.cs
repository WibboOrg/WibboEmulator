using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Rooms.Map.Movement;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;
using System.Drawing;
using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;

namespace Wibbo.Game.Items.Wired.Actions
{
    public class Chase : WiredActionBase, IWiredEffect, IWired
    {
        public Chase(Item item, Room room) : base(item, room, (int)WiredActionType.CHASE)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            foreach (Item roomItem in this.Items)
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
            
            RoomUser roomUser = this.RoomInstance.GetGameMap().SquareHasUserNear(item.X, item.Y);
            if (roomUser != null)
            {
                this.RoomInstance.GetWiredHandler().TriggerCollision(roomUser, item);
                return;
            }

            item.Movement = this.RoomInstance.GetGameMap().GetChasingMovement(item.X, item.Y, item.Movement);
            if (item.Movement == MovementState.none)
            {
                return;
            }

            Point newPoint = MovementUtility.HandleMovement(item.Coordinate, item.Movement);

            if (newPoint != item.Coordinate)
            {
                int oldX = item.X;
                int oldY = item.Y;
                double oldZ = item.Z;

                if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
                {
                    this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newPoint.X, newPoint.Y, item.Z, item.Id));
                }
            }
            return;
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
