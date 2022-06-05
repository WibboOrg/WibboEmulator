using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class HeightMapComposer : ServerPacket
    {
        public HeightMapComposer(RoomModelDynamic Map, double Height = 0.0)
            : base(ServerPacketHeader.ROOM_HEIGHT_MAP)
        {
            this.WriteInteger(Map.MapSizeX);
            this.WriteInteger(Map.MapSizeX * Map.MapSizeY);
            for (int i = 0; i < Map.MapSizeY; i++)
            {
                for (int j = 0; j < Map.MapSizeX; j++)
                {
                    if (Map.SqState[j, i] == SquareStateType.BLOCKED)
                    {
                        this.WriteShort(-1);
                    }
                    else
                    {
                        this.WriteShort((int)Math.Floor((Map.SqFloorHeight[j, i] + Height) * 256.0));
                    }
                }
            }
        }
    }
}
