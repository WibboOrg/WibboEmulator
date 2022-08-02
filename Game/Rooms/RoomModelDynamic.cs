using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using System.Text;

namespace WibboEmulator.Game.Rooms
{
    public class RoomModelDynamic
    {
        private RoomModel staticModel;

        public int DoorX;
        public int DoorY;
        public double DoorZ;
        public int DoorOrientation;
        public string Heightmap;

        public SquareStateType[,] SqState;
        public short[,] SqFloorHeight;
        public int MapSizeX;
        public int MapSizeY;
        private ServerPacket SerializedRelativeHeightmap;
        private bool RelativeSerialized;
        private ServerPacket SerializedHeightmap;
        private bool HeightmapSerialized;
        public int WallHeight;

        public RoomModelDynamic(RoomModel pModel)
        {
            this.staticModel = pModel;
            this.DoorX = this.staticModel.DoorX;
            this.DoorY = this.staticModel.DoorY;
            this.DoorZ = this.staticModel.DoorZ;
            this.DoorOrientation = this.staticModel.DoorOrientation;
            this.Heightmap = this.staticModel.Heightmap;
            this.MapSizeX = this.staticModel.MapSizeX;
            this.MapSizeY = this.staticModel.MapSizeY;
            this.WallHeight = this.staticModel.WallHeight;
            this.Generate();
        }

        public void Generate()
        {
            this.SqState = new SquareStateType[this.MapSizeX, this.MapSizeY];
            this.SqFloorHeight = new short[this.MapSizeX, this.MapSizeY];
            for (int index1 = 0; index1 < this.MapSizeY; ++index1)
            {
                for (int index2 = 0; index2 < this.MapSizeX; ++index2)
                {
                    if (index2 > this.staticModel.MapSizeX - 1 || index1 > this.staticModel.MapSizeY - 1)
                    {
                        this.SqState[index2, index1] = SquareStateType.BLOCKED;
                    }
                    else
                    {
                        this.SqState[index2, index1] = this.staticModel.SqState[index2, index1];
                        this.SqFloorHeight[index2, index1] = this.staticModel.SqFloorHeight[index2, index1];
                    }
                }
            }
            this.RelativeSerialized = false;
            this.HeightmapSerialized = false;
        }

        public void SetUpdateState()
        {
            this.RelativeSerialized = false;
            this.HeightmapSerialized = false;
        }

        public ServerPacket SerializeRelativeHeightmap()
        {
            if (!this.RelativeSerialized)
            {
                this.SerializedRelativeHeightmap = new HeightMapComposer(this);
                this.RelativeSerialized = true;
            }

            return this.SerializedRelativeHeightmap;
        }

        public ServerPacket GetHeightmap()
        {
            if (!this.HeightmapSerialized)
            {
                this.SerializedHeightmap = this.SerializeHeightmap();
                this.HeightmapSerialized = true;
            }

            return this.SerializedHeightmap;
        }

        private ServerPacket SerializeHeightmap()
        {
            StringBuilder thatMessage = new StringBuilder();

            for (int y = 0; y < this.MapSizeY; y++)
            {
                for (int x = 0; x < this.MapSizeX; x++)
                {
                    if (x == this.DoorX && y == this.DoorY)
                    {
                        thatMessage.Append(this.Parse(Convert.ToInt16(this.DoorZ)));
                    }
                    else if (this.SqState[x, y] == SquareStateType.BLOCKED)
                    {
                        thatMessage.Append('x');
                    }
                    else
                    {
                        thatMessage.Append(this.Parse(this.SqFloorHeight[x, y]));
                    }
                }

                thatMessage.Append(Convert.ToChar(13));
            }

            return new FloorHeightMapComposer(this.WallHeight, thatMessage.ToString());
        }

        private string Parse(short text)
        {
            switch (text)
            {
                case 10:
                    return "a";
                case 11:
                    return "b";
                case 12:
                    return "c";
                case 13:
                    return "d";
                case 14:
                    return "e";
                case 15:
                    return "f";
                case 16:
                    return "g";
                case 17:
                    return "h";
                case 18:
                    return "i";
                case 19:
                    return "j";
                case 20:
                    return "k";
                case 21:
                    return "l";
                case 22:
                    return "m";
                case 23:
                    return "n";
                case 24:
                    return "o";
                case 25:
                    return "p";
                case 26:
                    return "q";
                case 27:
                    return "r";
                case 28:
                    return "s";
                case 29:
                    return "t";
                case 30:
                    return "u";
                case 31:
                    return "v";
                case 32:
                    return "w";
            }
            return text.ToString();
        }

        public ServerPacket SetHeightMap(double Height)
        {
            return new HeightMapComposer(this, Height);
        }

        public void SetMapsize(int x, int y)
        {
            this.MapSizeX = x;
            this.MapSizeY = y;
            this.refreshArrays();
        }

        internal void refreshArrays()
        {
            SquareStateType[,] squareStateArray = new SquareStateType[this.MapSizeX, this.MapSizeY];
            short[,] numArray1 = new short[this.MapSizeX, this.MapSizeY];
            byte[,] numArray2 = new byte[this.MapSizeX, this.MapSizeY];
            for (int index1 = 0; index1 < this.MapSizeY; ++index1)
            {
                for (int index2 = 0; index2 < this.MapSizeX; ++index2)
                {
                    if (index2 > this.staticModel.MapSizeX - 1 || index1 > this.staticModel.MapSizeY - 1)
                    {
                        squareStateArray[index2, index1] = SquareStateType.BLOCKED;
                    }
                    else
                    {
                        squareStateArray[index2, index1] = this.SqState[index2, index1];
                        numArray1[index2, index1] = this.SqFloorHeight[index2, index1];
                    }
                }
            }
            this.SqState = squareStateArray;
            this.SqFloorHeight = numArray1;
            this.RelativeSerialized = false;
            this.HeightmapSerialized = false;
        }


        public void Destroy()
        {
            Array.Clear(this.SqState, 0, this.SqState.Length);
            Array.Clear(this.SqFloorHeight, 0, this.SqFloorHeight.Length);
            this.staticModel = null;
            this.Heightmap = null;
            this.SqState = null;
            this.SqFloorHeight = null;
        }
    }
}
