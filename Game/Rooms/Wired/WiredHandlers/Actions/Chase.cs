using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;
using System.Drawing;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class Chase : WiredActionBase, IWiredEffect, IWired
    {
        public Chase(Item item, Room room) : base(item, room, (int)WiredActionType.CHASE)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            foreach (Item roomItem in this.Items)
            {
                this.HandleMovement(roomItem);
            }
        }

        private void HandleMovement(Item item)
        {
            if (this.RoomInstance.GetRoomItemHandler().GetItem(item.Id) == null)
            {
                return;
            }
            
            RoomUser roomUser = this.RoomInstance.GetGameMap().SquareHasUserNear(item.GetX, item.GetY);
            if (roomUser != null)
            {
                this.RoomInstance.GetWiredHandler().TriggerCollision(roomUser, item);
                return;
            }

            item.movement = this.RoomInstance.GetGameMap().GetChasingMovement(item.GetX, item.GetY, item.movement);
            if (item.movement == MovementState.none)
            {
                return;
            }

            Point newPoint = MovementManagement.HandleMovement(item.Coordinate, item.movement);

            if (newPoint != item.Coordinate)
            {
                int OldX = item.GetX;
                int OldY = item.GetY;
                double OldZ = item.GetZ;

                if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
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
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
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
