using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Rooms.Map.Movement;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;
using System.Drawing;
using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;

namespace Wibbo.Game.Items.Wired.Actions
{
    public class MoveRotate : WiredActionBase, IWiredEffect, IWired
    {
        public MoveRotate(Item item, Room room) : base(item, room, (int)WiredActionType.MOVE_FURNI)
        {
            this.IntParams.Add(0);
            this.IntParams.Add(0);
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
            
            int movement = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int rotation = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

            Point newPoint = MovementUtility.HandleMovement(item.Coordinate, (MovementState)movement);
            int newRot = MovementUtility.HandleRotation(item.Rotation, (RotationState)rotation);

            if (newPoint != item.Coordinate || newRot != item.Rotation)
            {
                int oldX = item.X;
                int oldY = item.Y;
                double oldZ = item.Z;
                if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, newRot, false, false, (newRot != item.Rotation)))
                {
                    this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newPoint.X, newPoint.Y, item.Z, item.Id));
                }
            }

            return;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int movement = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int rotation = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

            string rotAndMove = (int)rotation + ";" + (int)movement;
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, rotAndMove, string.Empty, false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            int delay;
            if (int.TryParse(row["delay"].ToString(), out delay))
	            this.Delay = delay;

            if (int.TryParse(row["trigger_data"].ToString(), out delay))
                this.Delay = delay;

            string triggerData2 = row["trigger_data_2"].ToString();
            if (triggerData2.Contains(';'))
            {
                if(int.TryParse(triggerData2.Split(';')[1], out int movement))
                    this.IntParams.Add(movement);
                if(int.TryParse(triggerData2.Split(';')[0], out int rotationint))
                    this.IntParams.Add(rotationint);
            }

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
