namespace WibboEmulator.Games.Items;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Rooms.Games.Freeze;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.Map.Movement;

public class Item : IEquatable<Item>
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int BaseItemId { get; set; }
    public string ExtraData { get; set; }
    public int Extra { get; set; }
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
    public ItemData Data { get; set; }
    public FurniInteractor Interactor { get; set; }
    public List<Point> GetAffectedTiles { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public double Z { get; private set; }
    public Point Coordinate => new(this.X, this.Y);
    public double TotalHeight => this.Z + this.Height;
    public bool IsWallItem { get; set; }
    public bool IsFloorItem { get; set; }
    public int TeleLinkId { get; set; }

    public event EventHandler<ItemTriggeredEventArgs> ItemTrigger;
    public event EventHandler<ItemTriggeredEventArgs> OnUserWalksOffFurni;
    public event EventHandler<ItemTriggeredEventArgs> OnUserWalksOnFurni;

    public double Height
    {
        get
        {
            if (this.ItemData.AdjustableHeights.Count > 1 && this.ExtraData != "")
            {
                if (int.TryParse(this.ExtraData, out var index))
                {
                    if (index < this.ItemData.AdjustableHeights.Count && index >= 0)
                    {
                        if (index < this.ItemData.AdjustableHeights.Count && index >= 0)
                        {
                            return this.ItemData.AdjustableHeights[index];
                        }
                    }
                }
            }

            return this.Data.Height;
        }
    }

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
            else if (this.Rotation == 7)
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

    public ItemCategoryType Category => this.ItemData.InteractionType switch
    {
        InteractionType.GIFT or InteractionType.LEGEND_BOX or InteractionType.BADGE_BOX or InteractionType.LOOTBOX_2022 or InteractionType.DELUXE_BOX or InteractionType.EXTRA_BOX or
        InteractionType.CASE_MIEL or InteractionType.CASE_ATHENA or InteractionType.CASE_CELESTE or InteractionType.BAG_SAKURA or InteractionType.BAG_ATLANTA or InteractionType.BAG_KYOTO or InteractionType.BAG_NOEL or InteractionType.BAG_PORCELAINE => ItemCategoryType.PRESENT,

        InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE => ItemCategoryType.GUILD_FURNI,

        InteractionType.LANDSCAPE => ItemCategoryType.LANDSCAPE,

        InteractionType.FLOOR => ItemCategoryType.FLOOR,

        InteractionType.WALLPAPER => ItemCategoryType.WALL_PAPER,

        InteractionType.TROPHY => ItemCategoryType.TROPHY,

        _ => ItemCategoryType.DEFAULT,
    };

    public Item(int id, int roomId, int baseItemId, string extraData, int limitedNumber, int limitedStack, int x, int y, double z, int rot,
        string wallCoord, Room room)
    {
        if (ItemManager.GetItem(baseItemId, out var data))
        {
            this.Id = id;
            this.RoomId = roomId;
            this.BaseItemId = baseItemId;
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
            this.Data = data;
            this.WallCoord = wallCoord;
            this.Scores = [];
            this.EffectId = this.Data.EffectId;

            if (this.ItemData == null)
            {
                ExceptionLogger.LogException("Unknown baseID: " + baseItemId);
                return;
            }

            switch (this.ItemData.InteractionType)
            {
                case InteractionType.FOOTBALL_COUNTER_GREEN:
                case InteractionType.BANZAI_GATE_GREEN:
                case InteractionType.BANZAI_SCORE_GREEN:
                case InteractionType.FREEZE_GREEN_COUNTER:
                case InteractionType.FREEZE_GREEN_GATE:
                    this.Team = TeamType.Green;
                    break;
                case InteractionType.FOOTBALL_COUNTER_YELLOW:
                case InteractionType.BANZAI_GATE_YELLOW:
                case InteractionType.BANZAI_SCORE_YELLOW:
                case InteractionType.FREEZE_YELLOW_COUNTER:
                case InteractionType.FREEZE_YELLOW_GATE:
                    this.Team = TeamType.Yellow;
                    break;
                case InteractionType.FOOTBALL_COUNTER_BLUE:
                case InteractionType.BANZAI_GATE_BLUE:
                case InteractionType.BANZAI_SCORE_BLUE:
                case InteractionType.FREEZE_BLUE_COUNTER:
                case InteractionType.FREEZE_BLUE_GATE:
                    this.Team = TeamType.Blue;
                    break;
                case InteractionType.FOOTBALL_COUNTER_RED:
                case InteractionType.BANZAI_GATE_RED:
                case InteractionType.BANZAI_SCORE_RED:
                case InteractionType.FREEZE_RED_COUNTER:
                case InteractionType.FREEZE_RED_GATE:
                    this.Team = TeamType.Red;
                    break;
                case InteractionType.BANZAI_TELE:
                    this.ExtraData = "";
                    break;
                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    if (!string.IsNullOrEmpty(extraData) && extraData.Contains(';'))
                    {
                        if (int.TryParse(this.ExtraData.Split(';')[1], out var groupId))
                        {
                            this.GroupId = groupId;
                        }
                    }
                    break;
                case InteractionType.GIFT:
                {
                    if (!string.IsNullOrEmpty(extraData) && extraData.Contains(';') && extraData.Contains(Convert.ToChar(5)))
                    {
                        var giftData = this.ExtraData.Split(';', 2);
                        var giftExtraData = giftData[1].Split(Convert.ToChar(5));
                        var giftRibbon = int.Parse(giftExtraData[1]);
                        var giftBoxId = int.Parse(giftExtraData[2]);

                        this.Extra = (giftBoxId * 1000) + giftRibbon;
                    }
                    break;
                }
            }
            this.IsWallItem = this.ItemData.Type == ItemType.I;
            this.IsFloorItem = this.ItemData.Type == ItemType.S;

            if (room == null)
            {
                return;
            }

            this.Room = room;
            this.GetAffectedTiles = GameMap.GetAffectedTiles(this.ItemData.Length, this.ItemData.Width, this.X, this.Y, rot);
            this.Interactor = ItemFactory.CreateInteractor(this);
        }
    }

    public void SetTeleLinkId(int teleLinkId) => this.TeleLinkId = teleLinkId;

    public void SetState(int x, int y, double z, bool updateTiles = false)
    {
        this.X = x;
        this.Y = y;
        if (!double.IsInfinity(z))
        {
            this.Z = z;
        }

        if (updateTiles)
        {
            this.GetAffectedTiles = GameMap.GetAffectedTiles(this.ItemData.Length, this.ItemData.Width, this.X, this.Y, this.Rotation);
        }
    }

    public void OnTrigger(RoomUser user)
    {
        if (this.ItemTrigger == null)
        {
            return;
        }

        this.ItemTrigger(null, new ItemTriggeredEventArgs(user, this));
    }

    public void Destroy()
    {
        this.GetAffectedTiles.Clear();

        this.WiredHandler?.Dispose();

        this.Room = null;
        this.WiredHandler = null;

        this.ItemTrigger = null;
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
                if (!this.Room.GameMap.CanStackItem(x + 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x - 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.downleft;
                }
                else
                {
                    if (this.Room.GameMap.CanStackItem(x + 1, y + 1, true) && this.Room.GameMap.CanStackItem(x - 1, y - 1, true))
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
                        if (this.Room.GameMap.CanStackItem(x - 1, y - 1, true) && !this.Room.GameMap.CanStackItem(x + 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upleft;
                        }
                        else if (this.Room.GameMap.CanStackItem(x + 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x - 1, y - 1, true))
                        {
                            this.MovementDir = MovementDirection.downright;
                        }
                    }
                }
                break;
            case MovementDirection.upleft:
                if (!this.Room.GameMap.CanStackItem(x - 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x + 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.downright;
                }
                else
                {
                    if (this.Room.GameMap.CanStackItem(x - 1, y + 1, true) && this.Room.GameMap.CanStackItem(x + 1, y - 1, true))
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
                        if (this.Room.GameMap.CanStackItem(x + 1, y - 1, true) && !this.Room.GameMap.CanStackItem(x - 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upright;
                        }
                        else if (this.Room.GameMap.CanStackItem(x - 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x + 1, y - 1, true))
                        {
                            this.MovementDir = MovementDirection.downleft;
                        }
                    }
                }
                break;
            case MovementDirection.downright:
                if (!this.Room.GameMap.CanStackItem(x - 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x + 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.upleft;
                }
                else
                {
                    if (this.Room.GameMap.CanStackItem(x - 1, y + 1, true) && this.Room.GameMap.CanStackItem(x + 1, y - 1, true))
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
                        if (this.Room.GameMap.CanStackItem(x + 1, y - 1, true) && !this.Room.GameMap.CanStackItem(x - 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upright;
                        }
                        else if (this.Room.GameMap.CanStackItem(x - 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x + 1, y - 1, true))
                        {
                            this.MovementDir = MovementDirection.downleft;
                        }
                    }
                }
                break;
            case MovementDirection.downleft:
                if (!this.Room.GameMap.CanStackItem(x + 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x - 1, y - 1, true))
                {
                    this.MovementDir = MovementDirection.upright;
                }
                else
                {
                    if (this.Room.GameMap.CanStackItem(x + 1, y + 1, true) && this.Room.GameMap.CanStackItem(x - 1, y - 1, true))
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
                        if (this.Room.GameMap.CanStackItem(x - 1, y - 1, true) && !this.Room.GameMap.CanStackItem(x + 1, y + 1, true))
                        {
                            this.MovementDir = MovementDirection.upleft;
                        }
                        else if (this.Room.GameMap.CanStackItem(x + 1, y + 1, true) && !this.Room.GameMap.CanStackItem(x - 1, y - 1, true))
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
        this.Room.RoomItemHandling.QueueRoomItemUpdate(this);
    }

    public void UpdateState(bool inDb = true)
    {
        if (this.Room == null)
        {
            return;
        }

        if (inDb)
        {
            this.Room.RoomItemHandling.UpdateItem(this);
        }

        if (this.IsFloorItem)
        {
            this.Room.SendPacket(new ObjectUpdateComposer(this, this.Room.RoomData.OwnerId));
        }
        else
        {
            this.Room.SendPacket(new ItemUpdateComposer(this, this.Room.RoomData.OwnerId));
        }
    }

    public void ResetBaseItem(Room room)
    {
        this.Data = null;
        this.Data = this.ItemData;
        this.Room = room;

        switch (this.ItemData.InteractionType)
        {
            case InteractionType.FOOTBALL_COUNTER_GREEN:
            case InteractionType.BANZAI_GATE_GREEN:
            case InteractionType.BANZAI_SCORE_GREEN:
            case InteractionType.FREEZE_GREEN_COUNTER:
            case InteractionType.FREEZE_GREEN_GATE:
                this.Team = TeamType.Green;
                break;
            case InteractionType.FOOTBALL_COUNTER_YELLOW:
            case InteractionType.BANZAI_GATE_YELLOW:
            case InteractionType.BANZAI_SCORE_YELLOW:
            case InteractionType.FREEZE_YELLOW_COUNTER:
            case InteractionType.FREEZE_YELLOW_GATE:
                this.Team = TeamType.Yellow;
                break;
            case InteractionType.FOOTBALL_COUNTER_BLUE:
            case InteractionType.BANZAI_GATE_BLUE:
            case InteractionType.BANZAI_SCORE_BLUE:
            case InteractionType.FREEZE_BLUE_COUNTER:
            case InteractionType.FREEZE_BLUE_GATE:
                this.Team = TeamType.Blue;
                break;
            case InteractionType.FOOTBALL_COUNTER_RED:
            case InteractionType.BANZAI_GATE_RED:
            case InteractionType.BANZAI_SCORE_RED:
            case InteractionType.FREEZE_RED_COUNTER:
            case InteractionType.FREEZE_RED_GATE:
                this.Team = TeamType.Red;
                break;
            case InteractionType.BANZAI_TELE:
                this.ExtraData = "";
                break;
            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                if (!string.IsNullOrEmpty(this.ExtraData))
                {
                    if (this.ExtraData.Contains(';'))
                    {
                        if (int.TryParse(this.ExtraData.Split(';')[1], out var groupId))
                        {
                            this.GroupId = groupId;
                        }
                    }
                }
                break;
        }

        this.GetAffectedTiles = GameMap.GetAffectedTiles(this.ItemData.Length, this.ItemData.Width, this.X, this.Y, this.Rotation);
        this.Interactor = ItemFactory.CreateInteractor(this);
    }

    public ItemData ItemData
    {
        get
        {
            if (this.Data == null)
            {
                if (ItemManager.GetItem(this.BaseItemId, out var itemData))
                {
                    this.Data = itemData;
                }
            }

            return this.Data;
        }
    }

    public Room Room { get; private set; }

    public void UserWalksOnFurni(RoomUser user, Item item) => this.OnUserWalksOnFurni?.Invoke(this, new(user, item));

    public void UserWalksOffFurni(RoomUser user, Item item) => this.OnUserWalksOffFurni?.Invoke(this, new(user, item));

    public override bool Equals(object obj) => this.Equals(obj as Item);

    public override int GetHashCode() => this.Id;
}
