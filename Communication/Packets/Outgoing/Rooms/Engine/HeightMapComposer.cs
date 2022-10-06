namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

internal class HeightMapComposer : ServerPacket
{
    public HeightMapComposer(RoomModelDynamic map, double height = 0.0)
        : base(ServerPacketHeader.ROOM_HEIGHT_MAP)
    {
        this.WriteInteger(map.MapSizeX);
        this.WriteInteger(map.MapSizeX * map.MapSizeY);
        for (var i = 0; i < map.MapSizeY; i++)
        {
            for (var j = 0; j < map.MapSizeX; j++)
            {
                if (map.SqState[j, i] == SquareStateType.BLOCKED)
                {
                    this.WriteShort(-1);
                }
                else
                {
                    this.WriteShort((int)Math.Floor((map.SqFloorHeight[j, i] + height) * 256.0));
                }
            }
        }
    }
}
