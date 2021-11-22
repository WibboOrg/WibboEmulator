using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class MoveRotate : IWiredEffect, IWired, IWiredCycleable
    {
        private Room room;
        private WiredHandler handler;
        private readonly int itemID;
        private MovementState movement;
        private RotationState rotation;
        private List<Item> items;
        public int Delay { get; set; }
        private bool isDisposed;

        public MoveRotate(int movement, int rotation, List<Item> items, int delay, Room room, WiredHandler handler, int itemID)
        {
            this.movement = (MovementState)movement;
            this.rotation = (RotationState)rotation;
            this.items = items;
            this.Delay = delay;
            this.room = room;
            this.handler = handler;
            this.itemID = itemID;
            this.isDisposed = false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            }
            else
            {
                this.HandleItems();
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleItems();
            return false;
        }

        public void Dispose()
        {
            this.isDisposed = true;
            this.room = null;
            this.handler = null;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        private void HandleItems()
        {
            foreach (Item roomItem in this.items.ToArray())
            {
                this.HandleMovement(roomItem);
            }
        }

        private bool HandleMovement(Item item)
        {
            if (this.room.GetRoomItemHandler().GetItem(item.Id) == null)
            {
                return false;
            }

            Point newPoint = MovementManagement.HandleMovement(item.Coordinate, this.movement);
            int newRot = MovementManagement.HandleRotation(item.Rotation, this.rotation);


            if (newPoint != item.Coordinate || newRot != item.Rotation)
            {
                int OldX = item.GetX;
                int OldY = item.GetY;
                double OldZ = item.GetZ;
                if (this.room.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, newRot, false, false, (newRot != item.Rotation)))
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
                    this.room.SendPacket(Message);
                }
            }
            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string rotationandmove = (int)this.rotation + ";" + (int)this.movement;
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, rotationandmove, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.Delay = Convert.ToInt32(row["trigger_data"]);

            string triggerData2 = row["trigger_data_2"].ToString();
            string triggerItem = row["triggers_item"].ToString();

            if (triggerData2.Contains(";"))
            {
                int.TryParse(triggerData2.Split(';')[0], out int rotationint);

                int.TryParse(triggerData2.Split(';')[1], out int movementint);

                this.rotation = (RotationState)rotationint;
                this.movement = (MovementState)movementint;
            }

            if (triggerItem == "")
            {
                return;
            }

            foreach (string itemid in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(itemid));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.itemID)
                {
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(10);
            Message.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
            {
                Message.WriteInteger(roomItem.Id);
            }

            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(2);
            Message.WriteInteger((int)this.movement);
            Message.WriteInteger((int)this.rotation);
            Message.WriteInteger(0);
            Message.WriteInteger(4);
            Message.WriteInteger(this.Delay);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
