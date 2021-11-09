using Butterfly.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanFloorMapMessageComposer : ServerPacket
    {
        public FloorPlanFloorMapMessageComposer(ICollection<Item> Items)
            : base(ServerPacketHeader.ROOM_MODEL_BLOCKED_TILES)

        {
            WriteInteger(Items.Count);
            foreach (Item Item in Items.ToList())
            {
                WriteInteger(Item.GetX);
                WriteInteger(Item.GetY);
            }

        }
    }
}
