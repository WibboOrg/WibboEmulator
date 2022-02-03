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
        public SlideObjectBundleComposer(int x, int y, int nextX, int nextY, double nextHeight, Item item, int rollerId, double height = 0.0, int unitId = 0)
            : base(ServerPacketHeader.ROOM_ROLLING)
        {
            WriteInteger(x);
            WriteInteger(y);
            WriteInteger(nextX);
            WriteInteger(nextY);

            WriteInteger(item != null ? 1 : 0);
            if (item != null)
            {
                WriteInteger(item.Id);
                WriteString(item.Z.ToString());
                WriteString(nextHeight.ToString());
            }

            WriteInteger(rollerId);

            if (unitId > 0)
            {
                WriteInteger(2);
                WriteInteger(unitId);
                WriteString(height.ToString());
                WriteString(nextHeight.ToString());
            }
        }
        public SlideObjectBundleComposer(int x, int y, int nextX, int nextY, double nextHeight, ItemTemp item, int rollerId, int unitId = 0, double height = 0.0)
            : base(ServerPacketHeader.ROOM_ROLLING)
        {
            WriteInteger(x);
            WriteInteger(y);
            WriteInteger(nextX);
            WriteInteger(nextY);

            WriteInteger(item != null ? 1 : 0);
            if (item != null)
            {
                WriteInteger(item.Id);
                WriteString(item.Z.ToString());
                WriteString(nextHeight.ToString());
            }

            WriteInteger(rollerId);

            if (unitId > 0)
            {
                WriteInteger(2);
                WriteInteger(unitId);
                WriteString(height.ToString());
                WriteString(nextHeight.ToString());
            }
        }
    }
}
