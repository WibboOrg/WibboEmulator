using Butterfly.Game.Items;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer(int x, int y, int nextX, int nextY, double nextHeight, List<Item> items, int rollerId, int movementType, int unitId = 0, double height = 0.0)
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
        }
    }
}
