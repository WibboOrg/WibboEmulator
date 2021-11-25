using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Data;
using System.Drawing;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Wired.Actions
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

            item.movement = this.RoomInstance.GetGameMap().GetChasingMovement(item.X, item.Y, item.movement);
            if (item.movement == MovementState.none)
            {
                return;
            }

            Point newPoint = MovementUtility.HandleMovement(item.Coordinate, item.movement);

            if (newPoint != item.Coordinate)
            {
                int OldX = item.X;
                int OldY = item.Y;
                double OldZ = item.Z;

                if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
                {
                    ServerPacket Message = new ServerPacket(ServerPacketHeader.ROOM_ROLLING);
                    Message.WriteInteger(OldX);
                    Message.WriteInteger(OldY);
                    Message.WriteInteger(newPoint.X);
                    Message.WriteInteger(newPoint.Y);
                    Message.WriteInteger(1);
                    Message.WriteInteger(item.Id);
                    Message.WriteString(OldZ.ToString());
                    Message.WriteString(item.Z.ToString());
                    Message.WriteInteger(0);

                    this.RoomInstance.SendPacket(Message);
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
