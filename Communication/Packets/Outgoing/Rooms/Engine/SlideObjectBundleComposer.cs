using Butterfly.Game.Items;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer(int x, int y, double z, int nextX, int nextY, double nextHeight, int id, int rollerId = 0, bool isItem = true)
            : base(ServerPacketHeader.ROOM_ROLLING)
        {
            WriteInteger(x);
            WriteInteger(y);
            WriteInteger(nextX);
            WriteInteger(nextY);

            WriteInteger(isItem ? 1 : 0);
            if (isItem)
            {
                WriteInteger(id);
                WriteString(z.ToString());
                WriteString(nextHeight.ToString());
            }

            WriteInteger(rollerId);

            if (isItem) return;

            WriteInteger(2);
            WriteInteger(id);
            WriteString(z.ToString());
            WriteString(nextHeight.ToString());
        }
    }
}
