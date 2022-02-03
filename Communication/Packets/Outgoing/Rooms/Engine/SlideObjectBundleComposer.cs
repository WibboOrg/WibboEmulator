using Butterfly.Game.Items;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer(int x, int y, int nextX, int nextY, double nextHeight, List<Item> items, int rollerId, int unitId = 0, double height = 0.0)
            : base(ServerPacketHeader.ROOM_ROLLING)
        {
            WriteInteger(x);
            WriteInteger(y);
            WriteInteger(nextX);
            WriteInteger(nextY);

            WriteInteger(items.Count);
            foreach(Item item in items)
            {
                WriteInteger(item.Id);
                WriteString(item.Height.ToString());
                WriteString(nextHeight.ToString());
            }

            WriteInteger(rollerId);

            if(unitId > 0)
            {
                WriteInteger(2);
                WriteInteger(unitId);
                WriteString(Convert.ToString(height));
                WriteString(Convert.ToString(nextHeight));
            }
        }
        public SlideObjectBundleComposer(int x, int y, int nextX, int nextY, double nextHeight, Item Item, int rollerId, double height = 0.0, int unitId = 0)
            : base(ServerPacketHeader.ROOM_ROLLING)
        {
            WriteInteger(x);
            WriteInteger(y);
            WriteInteger(nextX);
            WriteInteger(nextY);

            WriteInteger(1);
            WriteInteger(Item.Id);
            WriteString(Item.Height.ToString());
            WriteString(nextHeight.ToString());

            WriteInteger(rollerId);

            if (unitId > 0)
            {
                WriteInteger(2);
                WriteInteger(unitId);
                WriteString(Convert.ToString(height));
                WriteString(Convert.ToString(nextHeight));
            }
        }
        public SlideObjectBundleComposer(int x, int y, int nextX, int nextY, double nextHeight, ItemTemp Item, int rollerId, int unitId = 0, double height = 0.0)
            : base(ServerPacketHeader.ROOM_ROLLING)
        {
            WriteInteger(x);
            WriteInteger(y);
            WriteInteger(nextX);
            WriteInteger(nextY);

            WriteInteger(1);
            WriteInteger(Item.Id);
            WriteString(Item.Z.ToString());
            WriteString(nextHeight.ToString());

            WriteInteger(rollerId);

            if (unitId > 0)
            {
                WriteInteger(2);
                WriteInteger(unitId);
                WriteString(Convert.ToString(height));
                WriteString(Convert.ToString(nextHeight));
            }
        }
    }
}
