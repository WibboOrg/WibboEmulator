namespace WibboEmulator.Games.Rooms;
using System.Text;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Rooms.Map;

public class RoomModelDynamic
{
    private RoomModel _staticModel;

    public int DoorX { get; set; }
    public int DoorY { get; set; }
    public double DoorZ { get; set; }
    public int DoorOrientation { get; set; }
    public string Heightmap { get; set; }

    public SquareStateType[,] SqState { get; set; }
    public short[,] SqFloorHeight { get; set; }
    public int MapSizeX { get; set; }
    public int MapSizeY { get; set; }
    public int WallHeight { get; set; }

    private ServerPacket _serializedRelativeHeightmap;
    private bool _relativeSerialized;
    private ServerPacket _serializedHeightmap;
    private bool _heightmapSerialized;

    public RoomModelDynamic(RoomModel model)
    {
        this._staticModel = model;
        this.DoorX = this._staticModel.DoorX;
        this.DoorY = this._staticModel.DoorY;
        this.DoorZ = this._staticModel.DoorZ;
        this.DoorOrientation = this._staticModel.DoorOrientation;
        this.Heightmap = this._staticModel.Heightmap;
        this.MapSizeX = this._staticModel.MapSizeX;
        this.MapSizeY = this._staticModel.MapSizeY;
        this.WallHeight = this._staticModel.WallHeight;
        this.Generate();
    }

    public void Generate()
    {
        this.SqState = new SquareStateType[this.MapSizeX, this.MapSizeY];
        this.SqFloorHeight = new short[this.MapSizeX, this.MapSizeY];
        for (var index1 = 0; index1 < this.MapSizeY; ++index1)
        {
            for (var index2 = 0; index2 < this.MapSizeX; ++index2)
            {
                if (index2 > this._staticModel.MapSizeX - 1 || index1 > this._staticModel.MapSizeY - 1)
                {
                    this.SqState[index2, index1] = SquareStateType.BLOCKED;
                }
                else
                {
                    this.SqState[index2, index1] = this._staticModel.SqState[index2, index1];
                    this.SqFloorHeight[index2, index1] = this._staticModel.SqFloorHeight[index2, index1];
                }
            }
        }
        this._relativeSerialized = false;
        this._heightmapSerialized = false;
    }

    public void SetUpdateState()
    {
        this._relativeSerialized = false;
        this._heightmapSerialized = false;
    }

    public ServerPacket SerializeRelativeHeightmap()
    {
        if (!this._relativeSerialized)
        {
            this._serializedRelativeHeightmap = new HeightMapComposer(this);
            this._relativeSerialized = true;
        }

        return this._serializedRelativeHeightmap;
    }

    public ServerPacket GetHeightmap()
    {
        if (!this._heightmapSerialized)
        {
            this._serializedHeightmap = this.SerializeHeightmap();
            this._heightmapSerialized = true;
        }

        return this._serializedHeightmap;
    }

    private ServerPacket SerializeHeightmap()
    {
        var thatMessage = new StringBuilder();

        for (var y = 0; y < this.MapSizeY; y++)
        {
            for (var x = 0; x < this.MapSizeX; x++)
            {
                if (x == this.DoorX && y == this.DoorY)
                {
                    _ = thatMessage.Append(Parse(Convert.ToInt16(this.DoorZ)));
                }
                else if (this.SqState[x, y] == SquareStateType.BLOCKED)
                {
                    _ = thatMessage.Append('x');
                }
                else
                {
                    _ = thatMessage.Append(Parse(this.SqFloorHeight[x, y]));
                }
            }

            _ = thatMessage.Append(Convert.ToChar(13));
        }

        return new FloorHeightMapComposer(this.WallHeight, thatMessage.ToString());
    }

    private static string Parse(short text) => text switch
    {
        10 => "a",
        11 => "b",
        12 => "c",
        13 => "d",
        14 => "e",
        15 => "f",
        16 => "g",
        17 => "h",
        18 => "i",
        19 => "j",
        20 => "k",
        21 => "l",
        22 => "m",
        23 => "n",
        24 => "o",
        25 => "p",
        26 => "q",
        27 => "r",
        28 => "s",
        29 => "t",
        30 => "u",
        31 => "v",
        32 => "w",
        _ => text.ToString(),
    };

    public ServerPacket SetHeightMap(double height) => new HeightMapComposer(this, height);

    public void SetMapsize(int x, int y)
    {
        this.MapSizeX = x;
        this.MapSizeY = y;
        this.RefreshArrays();
    }

    internal void RefreshArrays()
    {
        var squareStateArray = new SquareStateType[this.MapSizeX, this.MapSizeY];
        var numArray1 = new short[this.MapSizeX, this.MapSizeY];

        _ = new byte[this.MapSizeX, this.MapSizeY];
        for (var index1 = 0; index1 < this.MapSizeY; ++index1)
        {
            for (var index2 = 0; index2 < this.MapSizeX; ++index2)
            {
                if (index2 > this._staticModel.MapSizeX - 1 || index1 > this._staticModel.MapSizeY - 1)
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
        this._relativeSerialized = false;
        this._heightmapSerialized = false;
    }


    public void Destroy()
    {
        Array.Clear(this.SqState, 0, this.SqState.Length);
        Array.Clear(this.SqFloorHeight, 0, this.SqFloorHeight.Length);
        this._staticModel = null;
        this.Heightmap = null;
        this.SqState = null;
        this.SqFloorHeight = null;
    }
}
