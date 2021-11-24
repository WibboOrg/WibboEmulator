using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;
using System.Drawing;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class MoveRotate : WiredActionBase, IWiredEffect, IWired
    {
        public MoveRotate(Item item, Room room) : base(item, room, (int)WiredActionType.MOVE_FURNI)
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
            
            int movement = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int rotation = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

            Point newPoint = MovementManagement.HandleMovement(item.Coordinate, (MovementState)movement);
            int newRot = MovementManagement.HandleRotation(item.Rotation, (RotationState)rotation);

            if (newPoint != item.Coordinate || newRot != item.Rotation)
            {
                int OldX = item.GetX;
                int OldY = item.GetY;
                double OldZ = item.GetZ;
                if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, newRot, false, false, (newRot != item.Rotation)))
                {
                    ServerPacket Message = new ServerPacket(ServerPacketHeader.ROOM_ROLLING);
                    Message.WriteInteger(OldX);
                    Message.WriteInteger(OldY);
                    Message.WriteInteger(newPoint.X);
                    Message.WriteInteger(newPoint.Y);
                    Message.WriteInteger(1);
                    Message.WriteInteger(item.Id);
                    Message.WriteString(OldZ.ToString().Replace(',', '.'));
                    Message.WriteString(item.GetZ.ToString().Replace(',', '.'));
                    Message.WriteInteger(0);
                    this.RoomInstance.SendPacket(Message);
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
            int delay = 0;
            if (int.TryParse(row["delay"].ToString(), out delay))
	            this.Delay = delay;

            if (int.TryParse(row["trigger_data"].ToString(), out delay))
                this.Delay = delay;

            string triggerData2 = row["trigger_data_2"].ToString();
            if (triggerData2.Contains(";"))
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
