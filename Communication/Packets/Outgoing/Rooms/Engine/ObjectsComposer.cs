using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.Wired;
using System;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectsComposer : ServerPacket
    {
        public ObjectsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.FURNITURE_FLOOR)
        {
            this.WriteInteger(1);

            this.WriteInteger(Room.RoomData.OwnerId);
            this.WriteString(Room.RoomData.OwnerName);

            this.WriteInteger(Objects.Length);
            foreach (Item Item in Objects)
            {
                this.WriteFloorItem(Item, Convert.ToInt32(Room.RoomData.OwnerId), Room.RoomData.HideWireds);
            }
        }

        public ObjectsComposer(ItemTemp[] Objects, Room Room)
            : base(ServerPacketHeader.FURNITURE_FLOOR)
        {
            this.WriteInteger(1);

            this.WriteInteger(Room.RoomData.OwnerId);
            this.WriteString(Room.RoomData.OwnerName);

            this.WriteInteger(Objects.Length);
            foreach (ItemTemp Item in Objects)
            {
                this.WriteFloorItem(Item, Convert.ToInt32(Room.RoomData.OwnerId));
            }
        }

        private void WriteFloorItem(ItemTemp Item, int UserID)
        {

            this.WriteInteger(Item.Id);
            this.WriteInteger(Item.SpriteId);
            this.WriteInteger(Item.X);
            this.WriteInteger(Item.Y);
            this.WriteInteger(2);
            this.WriteString(string.Format("{0:0.00}", Item.Z));
            this.WriteString(string.Empty);

            if (Item.InteractionType == InteractionTypeTemp.RPITEM)
            {
                this.WriteInteger(0);
                this.WriteInteger(1);

                this.WriteInteger(5);

                this.WriteString("state");
                this.WriteString("0");
                this.WriteString("imageUrl");
                this.WriteString("https://swf.wibbo.me/items/" + Item.ExtraData + ".png");
                this.WriteString("offsetX");
                this.WriteString("-20");
                this.WriteString("offsetY");
                this.WriteString("10");
                this.WriteString("offsetZ");
                this.WriteString("10002");
            }
            else
            {
                this.WriteInteger(1);
                this.WriteInteger(0);
                this.WriteString(Item.ExtraData);
            }

            this.WriteInteger(-1); // to-do: check
            this.WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
            this.WriteInteger(UserID);
        }

        private void WriteFloorItem(Item Item, int UserID, bool HideWired)
        {

            this.WriteInteger(Item.Id);
            this.WriteInteger((HideWired && WiredUtillity.TypeIsWired(Item.GetBaseItem().InteractionType) && (Item.GetBaseItem().InteractionType != InteractionType.HIGHSCORE && Item.GetBaseItem().InteractionType != InteractionType.HIGHSCOREPOINTS)) ? 31294061 : Item.GetBaseItem().SpriteId);
            this.WriteInteger(Item.GetX);
            this.WriteInteger(Item.GetY);
            this.WriteInteger(Item.Rotation);
            this.WriteString(string.Format("{0:0.00}", Item.GetZ));
            this.WriteString(string.Empty);

            if (Item.Limited > 0)
            {
                this.WriteInteger(1);
                this.WriteInteger(256);
                this.WriteString(Item.ExtraData);
                this.WriteInteger(Item.Limited);
                this.WriteInteger(Item.LimitedStack);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            this.WriteInteger(-1); // to-do: check
            this.WriteInteger(1); //(Item.GetBaseItem().Modes > 1) ? 1 : 0
            this.WriteInteger(UserID);
        }
    }
}
