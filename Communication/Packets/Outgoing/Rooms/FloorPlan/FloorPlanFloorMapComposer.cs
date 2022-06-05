using Butterfly.Game.Items;
using System.Collections.Concurrent;
using System.Drawing;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanFloorMapComposer : ServerPacket
    {
        public FloorPlanFloorMapComposer(ConcurrentDictionary<Point, List<Item>> CoordinatedItems)
            : base(ServerPacketHeader.ROOM_MODEL_BLOCKED_TILES)
        {
            this.WriteInteger(CoordinatedItems.Count);

            foreach (Point Coords in CoordinatedItems.Keys)
            {
                this.WriteInteger(Coords.X);
                this.WriteInteger(Coords.Y);
            }
        }
    }
}
