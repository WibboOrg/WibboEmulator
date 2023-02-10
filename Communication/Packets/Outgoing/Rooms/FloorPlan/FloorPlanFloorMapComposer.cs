namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.FloorPlan;
using System.Collections.Concurrent;
using System.Drawing;
using WibboEmulator.Games.Items;

internal sealed class FloorPlanFloorMapComposer : ServerPacket
{
    public FloorPlanFloorMapComposer(ConcurrentDictionary<Point, List<Item>> coordinatedItems)
        : base(ServerPacketHeader.ROOM_MODEL_BLOCKED_TILES)
    {
        this.WriteInteger(coordinatedItems.Count);

        foreach (var coords in coordinatedItems.Keys)
        {
            this.WriteInteger(coords.X);
            this.WriteInteger(coords.Y);
        }
    }
}
