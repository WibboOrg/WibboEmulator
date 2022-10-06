namespace WibboEmulator.Games.Items;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core;
using WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Freeze;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Games.Rooms.PathFinding;
using WibboEmulator.Utilities.Events;

public delegate void OnItemTrigger(object sender, ItemTriggeredEventArgs e);

public class Item : IEquatable<Item>
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int BaseItem { get; set; }

    public string ExtraData { get; set; }
    public int GroupId { get; set; }
    public int Limited { get; set; }
    public int LimitedStack { get; set; }
    public TeamType Team { get; set; }
    public int InteractionCountHelper { get; set; }
    public int Value { get; set; }
    public FreezePowerUp FreezePowerUp { get; set; }
    public int Rotation { get; set; }
    public string WallCoord { get; set; }
    public int UpdateCounter { get; set; }
    public int InteractingUser { get; set; }
    public int InteractingUser2 { get; set; }
    public int EffectId { get; set; }

    public MovementState Movement { get; set; }
    public MovementDirection MovementDir { get; set; }

    public Dictionary<string, int> Scores { get; set; }

    public IWired WiredHandler { get; set; }
    public event OnItemTrigger ItemTriggerEventHandler;
    public event UserAndItemDelegate OnUserWalksOffFurni;
    public event UserAndItemDelegate OnUserWalksOnFurni;

    private Room _roomInstance;

    public Dictionary<int, Coord> GetAffectedTiles { get; private set; }

    public int X { get; private set; }

    public int Y { get; private set; }

    public double Z { get; private set; }

    public Point Coordinate => new(this.X, this.Y);

    public List<Point> GetCoords
    {
        get
        {
            var list = new List<Point>
            {
                this.Coordinate
            };
            foreach (var threeDcoord in this.GetAffectedTiles.Values)
            {
                list.Add(new Point(threeDcoord.X, threeDcoord.Y));
            }

            return list;
        }
    }

    public double TotalHeight => this.Z + this.Height;

    public double Height
    {
        get
        {
            if (this.GetBaseItem().AdjustableHeights.Count > 1 && this.ExtraData != "")
            {
                if (int.TryParse(this.ExtraData, out var index))
                {
                    if (index < this.GetBaseItem().AdjustableHeights.Count && index >= 0)
                    {
                        if (index < this.GetBaseItem().AdjustableHeights.Count && index >= 0)
                        {
                            return this.GetBaseItem().AdjustableHeights[index];
                        }
                    }
                }
            }

            return this.Data.Height;
        }
    }

    public bool IsWallItem { get; set; }

    public bool IsFloorItem { get; set; }

    public Point SquareInFront
    {
        get
        {
            var point = new Point(this.X, this.Y);
            if (this.Rotation == 0)
            {
                point.Y--;
            }
            if (this.Rotation == 1)
            {
                point.X++;
                point.Y--;
            }
            else if (this.Rotation == 2)
            {
                point.X++;
            }
            else if (this.Rotation == 3)
            {
                point.X++;
                point.Y++;
            }
            else if (this.Rotation == 4)
            {
                point.Y++;
            }
            else if (this.Rotation == 5)
            {
                point.X--;
                point.Y++;
            }
            else if (this.Rotation == 6)
            {
                point.X--;
            }
            else if (this.Rotation == 6)
            {
                point.X--;
                point.Y--;
            }

            return point;
        }
    }

    public Point SquareBehind
    {
        get
        {
            var point = new Point(this.X, this.Y);
            if (this.Rotation == 0)
            {
                point.Y++;
            }
            if (this.Rotation == 1)
            {
                point.X--;
                point.Y++;
            }
            else if (this.Rotation == 2)
            {
                point.X--;
            }
            else if (this.Rotation == 3)
            {
                point.X--;
                point.Y--;
            }
            else if (this.Rotation == 4)
            {
                point.Y--;
            }
            else if (this.Rotation == 5)
            {
                point.X++;
                point.Y--;
            }
            else if (this.Rotation == 6)
            {
                point.X++;
            }
            else if (this.Rotation == 6)
            {
                point.X++;
                point.Y++;
            }

            return point;
        }
    }

    public ItemData Data { get; set; }
    public FurniInteractor Interactor { get; set; }

    public Item(int id, int roomId, int baseItem, string extraData, int limitedNumber, int limitedStack, int x, int y, double z, int rot,
        string wallCoord, Room room)
    {
        if (WibboEnvironment.GetGame().GetItemManager().GetItem(baseItem, out var Data))
        {
            this.Id = id;
            this.RoomId = roomId;
            this.BaseItem = baseItem;
            this.ExtraData = extraData;
            this.X = x;
            this.Y = y;
            if (!double.IsInfinity(z))
            {
                this.Z = z;
            }

            this.Rotation = rot;
            this.UpdateCounter = 0;
            this.InteractingUser = 0;
            this.InteractingUser2 = 0;
            this.InteractionCountHelper = 0;
            this.Value = 0;
            this.Limited = limitedNumber;
            this.LimitedStack = limitedStack;
            this.Data = Data;
            this.WallCoord = wallCoord;

            this.Scores = new Dictionary<string, int>();

            this.EffectId = this.Data.EffectId;

            this._roomInstance = room;
            if (this.GetBaseItem() == null)
            {
                ExceptionLogger.LogException("Unknown baseID: " + baseItem);
            }

            switch (this.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALLCOUNTERGREEN:
                case InteractionType.BANZAIGATEGREEN:
                case InteractionType.BANZAISCOREGREEN:
                case InteractionType.FREEZEGREENCOUNTER:
                case InteractionType.FREEZEGREENGATE:
                    this.Team = TeamType.GREEN;
                    break;
                case InteractionType.FOOTBALLCOUNTERYELLOW:
                case InteractionType.BANZAIGATEYELLOW:
                case InteractionType.BANZAISCOREYELLOW:
                case InteractionType.FREEZEYELLOWCOUNTER:
                case InteractionType.FREEZEYELLOWGATE:
                    this.Team = TeamType.YELLOW;
                    break;
                case InteractionType.FOOTBALLCOUNTERBLUE:
                case InteractionType.BANZAIGATEBLUE:
                case InteractionType.BANZAISCOREBLUE:
                case InteractionType.FREEZEBLUECOUNTER:
                case InteractionType.FREEZEBLUEGATE:
                    this.Team = TeamType.BLUE;
                    break;
                case InteractionType.FOOTBALLCOUNTERRED:
                case InteractionType.BANZAIGATERED:
                case InteractionType.BANZAISCORERED:
                case InteractionType.FREEZEREDCOUNTER:
                case InteractionType.FREEZEREDGATE:
                    this.Team = TeamType.RED;
                    break;
                case InteractionType.BANZAITELE:
                    this.ExtraData = "";
                    break;
                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    if (!string.IsNullOrEmpty(extraData))
                    {
                        if (extraData.Contains(';'))
                        {
                            if (int.TryParse(this.ExtraData.Split(new char[1] { ';' })[1], out var groupId))
                            {
                                this.GroupId = groupId;
                            }
                        }
                    }
                    break;
            }
            this.IsWallItem = this.GetBaseItem().Type.ToString().ToLower() == "i";
            this.IsFloorItem = this.GetBaseItem().Type.ToString().ToLower() == "s";

            this.GetAffectedTiles = Gamemap.GetAffectedTiles(this.GetBaseItem().Length, this.GetBaseItem().Width, this.X, this.Y, rot);

            this.Interactor = ItemFactory.CreateInteractor(this);
        }
    }

    public void SetState(int pX, int pY, double pZ, Dictionary<int, Coord> tiles)
    {
        this.X = pX;
        this.Y = pY;
        if (!double.IsInfinity(pZ))
        {
            this.Z = pZ;
        }

        this.GetAffectedTiles = tiles;
    }

    public void OnTrigger(RoomUser user)
    {
        if (this.ItemTriggerEventHandler == null)
        {
            return;
        }

        this.ItemTriggerEventHandler(null, new ItemTriggeredEventArgs(user, this));
    }

    public void Destroy()
    {
        this.GetAffectedTiles.Clear();

        if (this.WiredHandler != null)
        {
            this.WiredHandler.Dispose();
        }

        this._roomInstance = null;
        this.WiredHandler = null;

        this.ItemTriggerEventHandler = null;
        this.OnUserWalksOffFurni = null;
        this.OnUserWalksOnFurni = null;
    }

    public bool Equals(Item other) => other.Id == this.Id;

    public Point GetMoveCoord(int x, int y, int i)
    {

        switch (this.MovementDir)
        {
            case MovementDirection.up:
            {
                y -= i;
                break;
            }
            case MovementDirection.upright:
            {
                x += i;
                y -= i;
                break;
            }
            case MovementDirection.right:
            {
                x += i;
                break;
            }
            case MovementDirection.downright:
            {
                x += i;
                y += i;
                break;
            }
            case MovementDirection.down:
            {
                y += i;
                break;
            }
            case MovementDirection.downleft:
            {
                x -= i;
                y += i;
                break;
            }
            case MovementDirection.left:
            {
                x -= i;
                break;
            }
            case MovementDirection.upleft:
            {
                x -= i;
                y -= i;
                break;
            }
        }

        return new Point(x, y);
    }

    public void GetNewDir(int x, int y)
    {
        switch (this.MovementDir)
        {
            case MovementDirection.up:
                this.MovementDir = MovementDirection.down;
                break;
            case MovementDirection.down:
                this.MovementDir = MovementDirection.up;
                break;
            case MovementDirection.right:
                this.MovementDir = MovementDirection.left;
                break;
            case MovementDirection.left:
                this.MovementDir = MovementDirection.right;
                break;

            case MovementDirection.upright:
                if (!this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.downleft;
                }
                else
                {
                    if (this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true))
                    {
                        if (WibboEnvironment.GetRandomNumber(1, 2) == 1)
                        {
                            this.MovementDir = MovementDirection.downright;
                        }
                        else
                        {
                            this.MovementDir = MovementDirection.upleft;
                        }
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upleft;
                        }
                        else if (this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true))
                        {
                            this.MovementDir = MovementDirection.downright;
                        }
                    }
                }
                break;
            case MovementDirection.upleft:
                if (!this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.downright;
                }
                else
                {
                    if (this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true))
                    {
                        if (WibboEnvironment.GetRandomNumber(1, 2) == 1)
                        {
                            this.MovementDir = MovementDirection.downleft;
                        }
                        else
                        {
                            this.MovementDir = MovementDirection.upright;
                        }
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upright;
                        }
                        else if (this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true))
                        {
                            this.MovementDir = MovementDirection.downleft;
                        }
                    }
                }
                break;
            case MovementDirection.downright:
                if (!this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.upleft;
                }
                else
                {
                    if (this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true))
                    {
                        if (WibboEnvironment.GetRandomNumber(1, 2) == 1)
                        {
                            this.MovementDir = MovementDirection.downleft;
                        }
                        else
                        {
                            this.MovementDir = MovementDirection.upright;
                        }
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upright;
                        }
                        else if (this.GetRoom().GetGameMap().CanStackItem(x - 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x + 1, y - 1, true))
                        {
                            this.MovementDir = MovementDirection.downleft;
                        }
                    }
                }
                break;
            case MovementDirection.downleft:
                if (!this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.upright;
                }
                else
                {
                    if (this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true))
                    {
                        if (WibboEnvironment.GetRandomNumber(1, 2) == 1)
                        {
                            this.MovementDir = MovementDirection.downright;
                        }
                        else
                        {
                            this.MovementDir = MovementDirection.upleft;
                        }
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upleft;
                        }
                        else if (this.GetRoom().GetGameMap().CanStackItem(x + 1, y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(x - 1, y - 1, true))
                        {
                            this.MovementDir = MovementDirection.downright;
                        }
                    }
                }
                break;
        }
    }

    public void ProcessUpdates()
    {
        this.UpdateCounter--;
        if (this.UpdateCounter > 0)
        {
            return;
        }

        this.UpdateCounter = 0;

        this.Interactor.OnTick(this);
    }

    public void ReqUpdate(int cycles)
    {
        if (this.UpdateCounter > 0)
        {
            return;
        }

        this.UpdateCounter = cycles;
        this.GetRoom().GetRoomItemHandler().QueueRoomItemUpdate(this);
    }

    public void UpdateState() => this.UpdateState(true, true);

    public void UpdateState(bool inDb, bool inRoom)
    {
        if (this.GetRoom() == null)
        {
            return;
        }

        if (inDb)
        {
            this.GetRoom().GetRoomItemHandler().UpdateItem(this);
        }

        if (!inRoom)
        {
            return;
        }

        if (this.IsFloorItem)
        {
            this.GetRoom().SendPacket(new ObjectUpdateComposer(this, this.GetRoom().RoomData.OwnerId));
        }
        else
        {
            this.GetRoom().SendPacket(new ItemUpdateComposer(this, this.GetRoom().RoomData.OwnerId));
        }
    }

    public void ResetBaseItem()
    {
        this.Data = null;
        this.Data = this.GetBaseItem();

        switch (this.GetBaseItem().InteractionType)
        {
            case InteractionType.FOOTBALLCOUNTERGREEN:
            case InteractionType.BANZAIGATEGREEN:
            case InteractionType.BANZAISCOREGREEN:
            case InteractionType.FREEZEGREENCOUNTER:
            case InteractionType.FREEZEGREENGATE:
                this.Team = TeamType.GREEN;
                break;
            case InteractionType.FOOTBALLCOUNTERYELLOW:
            case InteractionType.BANZAIGATEYELLOW:
            case InteractionType.BANZAISCOREYELLOW:
            case InteractionType.FREEZEYELLOWCOUNTER:
            case InteractionType.FREEZEYELLOWGATE:
                this.Team = TeamType.YELLOW;
                break;
            case InteractionType.FOOTBALLCOUNTERBLUE:
            case InteractionType.BANZAIGATEBLUE:
            case InteractionType.BANZAISCOREBLUE:
            case InteractionType.FREEZEBLUECOUNTER:
            case InteractionType.FREEZEBLUEGATE:
                this.Team = TeamType.BLUE;
                break;
            case InteractionType.FOOTBALLCOUNTERRED:
            case InteractionType.BANZAIGATERED:
            case InteractionType.BANZAISCORERED:
            case InteractionType.FREEZEREDCOUNTER:
            case InteractionType.FREEZEREDGATE:
                this.Team = TeamType.RED;
                break;
            case InteractionType.BANZAITELE:
                this.ExtraData = "";
                break;
            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                if (!string.IsNullOrEmpty(this.ExtraData))
                {
                    if (this.ExtraData.Contains(';'))
                    {
                        if(int.TryParse(this.ExtraData.Split(new char[1] { ';' })[1], out var groupId))
                        {
                            this.GroupId = groupId;
                        }
                    }
                }
                break;
        }

        this.GetAffectedTiles = Gamemap.GetAffectedTiles(this.GetBaseItem().Length, this.GetBaseItem().Width, this.X, this.Y, this.Rotation);
    }

    public ItemData GetBaseItem()
    {
        if (this.Data == null)
        {
            if (WibboEnvironment.GetGame().GetItemManager().GetItem(this.BaseItem, out var itemData))
            {
                this.Data = itemData;
            }
        }

        return this.Data;
    }

    public Room GetRoom() => this._roomInstance;

    public void UserWalksOnFurni(RoomUser user, Item item)
    {
        if (this.OnUserWalksOnFurni == null)
        {
            return;
        }

        this.OnUserWalksOnFurni(user, item);
    }

    public void UserWalksOffFurni(RoomUser user, Item item)
    {
        if (this.OnUserWalksOffFurni == null)
        {
            return;
        }

        this.OnUserWalksOffFurni(user, item);
    }
}
