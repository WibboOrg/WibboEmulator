using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Core;
using Butterfly.Game.Items.Interactors;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Rooms.PathFinding;
using Butterfly.Game.Items.Wired.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using Butterfly.Utilities.Events;

namespace Butterfly.Game.Items
{
    public delegate void OnItemTrigger(object sender, ItemTriggeredArgs e);

    public class Item : IEquatable<Item>
    {
        public int Id;
        public int RoomId;
        public int BaseItem;

        public string ExtraData;
        public int GroupId;
        public int Limited;
        public int LimitedStack;
        public TeamType Team;
        public int InteractionCountHelper;
        public int Value;
        public FreezePowerUp FreezePowerUp;
        public int Rotation;
        public string WallCoord;
        public int UpdateCounter;
        public int InteractingUser;
        public int InteractingUser2;
        public int EffectId;

        public MovementState Movement;
        public MovementDirection MovementDir;

        public Dictionary<string, int> Scores;

        public IWired WiredHandler;
        public event OnItemTrigger ItemTriggerEventHandler;
        public event UserAndItemDelegate OnUserWalksOffFurni;
        public event UserAndItemDelegate OnUserWalksOnFurni;

        private Room _roomInstance;

        public Dictionary<int, Coord> GetAffectedTiles { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public double Z { get; private set; }

        public Point Coordinate => new Point(this.X, this.Y);

        public List<Point> GetCoords
        {
            get
            {
                List<Point> list = new List<Point>
                {
                    this.Coordinate
                };
                foreach (Coord threeDcoord in this.GetAffectedTiles.Values)
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
                    if (int.TryParse(this.ExtraData, out int index))
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

        public bool IsWallItem;

        public bool IsFloorItem;

        public Point SquareInFront
        {
            get
            {
                Point point = new Point(this.X, this.Y);
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
                Point point = new Point(this.X, this.Y);
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

        public ItemData Data;
        public FurniInteractor Interactor;

        public Item(int mId, int RoomId, int mBaseItem, string ExtraData, int limitedNumber, int limitedStack, int X, int Y, double Z, int Rot, string wallCoord, Room room)
        {
            if (ButterflyEnvironment.GetGame().GetItemManager().GetItem(mBaseItem, out ItemData Data))
            {
                this.Id = mId;
                this.RoomId = RoomId;
                this.BaseItem = mBaseItem;
                this.ExtraData = ExtraData;
                this.X = X;
                this.Y = Y;
                if (!double.IsInfinity(Z))
                {
                    this.Z = Z;
                }

                this.Rotation = Rot;
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
                    Logging.LogException("Unknown baseID: " + mBaseItem);
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
                        if (!string.IsNullOrEmpty(ExtraData))
                        {
                            if (ExtraData.Contains(";"))
                            {
                                int.TryParse(this.ExtraData.Split(new char[1] { ';' })[1], out this.GroupId);
                            }
                        }
                        break;
                }
                this.IsWallItem = this.GetBaseItem().Type.ToString().ToLower() == "i";
                this.IsFloorItem = this.GetBaseItem().Type.ToString().ToLower() == "s";

                this.GetAffectedTiles = Gamemap.GetAffectedTiles(this.GetBaseItem().Length, this.GetBaseItem().Width, this.X, this.Y, Rot);

                this.Interactor = ItemFactory.CreateInteractor(this);
            }
        }

        public void SetState(int pX, int pY, double pZ, Dictionary<int, Coord> Tiles)
        {
            this.X = pX;
            this.Y = pY;
            if (!double.IsInfinity(pZ))
            {
                this.Z = pZ;
            }

            this.GetAffectedTiles = Tiles;
        }

        public void OnTrigger(RoomUser user)
        {
            if (this.ItemTriggerEventHandler == null)
            {
                return;
            }

            this.ItemTriggerEventHandler(null, new ItemTriggeredArgs(user, this));
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

        public bool Equals(Item comparedItem)
        {
            return comparedItem.Id == this.Id;
        }

        public Point GetMoveCoord(int X, int Y, int i)
        {

            switch (this.MovementDir)
            {
                case MovementDirection.up:
                    {
                        Y -= i;
                        break;
                    }
                case MovementDirection.upright:
                    {
                        X += i;
                        Y -= i;
                        break;
                    }
                case MovementDirection.right:
                    {
                        X += i;
                        break;
                    }
                case MovementDirection.downright:
                    {
                        X += i;
                        Y += i;
                        break;
                    }
                case MovementDirection.down:
                    {
                        Y += i;
                        break;
                    }
                case MovementDirection.downleft:
                    {
                        X -= i;
                        Y += i;
                        break;
                    }
                case MovementDirection.left:
                    {
                        X -= i;
                        break;
                    }
                case MovementDirection.upleft:
                    {
                        X -= i;
                        Y -= i;
                        break;
                    }
            }

            return new Point(X, Y);
        }

        public void GetNewDir(int X, int Y)
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
                    if (!this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.downleft;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                        {
                            if (ButterflyEnvironment.GetRandomNumber(1, 2) == 1)
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
                            if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true))
                            {
                                this.MovementDir = MovementDirection.upleft;
                            }
                            else if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                            {
                                this.MovementDir = MovementDirection.downright;
                            }
                        }
                    }
                    break;
                case MovementDirection.upleft:
                    if (!this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.downright;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                        {
                            if (ButterflyEnvironment.GetRandomNumber(1, 2) == 1)
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
                            if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true))
                            {
                                this.MovementDir = MovementDirection.upright;
                            }
                            else if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                            {
                                this.MovementDir = MovementDirection.downleft;
                            }
                        }
                    }
                    break;
                case MovementDirection.downright:
                    if (!this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.upleft;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                        {
                            if (ButterflyEnvironment.GetRandomNumber(1, 2) == 1)
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
                            if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true))
                            {
                                this.MovementDir = MovementDirection.upright;
                            }
                            else if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                            {
                                this.MovementDir = MovementDirection.downleft;
                            }
                        }
                    }
                    break;
                case MovementDirection.downleft:
                    if (!this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.upright;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                        {
                            if (ButterflyEnvironment.GetRandomNumber(1, 2) == 1)
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
                            if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true))
                            {
                                this.MovementDir = MovementDirection.upleft;
                            }
                            else if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
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

        public void ReqUpdate(int Cycles)
        {
            if (this.UpdateCounter > 0)
            {
                return;
            }

            this.UpdateCounter = Cycles;
            this.GetRoom().GetRoomItemHandler().QueueRoomItemUpdate(this);
        }

        public void UpdateState()
        {
            this.UpdateState(true, true);
        }

        public void UpdateState(bool inDb, bool inRoom)
        {
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
                        if (this.ExtraData.Contains(";"))
                        {
                            int.TryParse(this.ExtraData.Split(new char[1] { ';' })[1], out this.GroupId);
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
                if (ButterflyEnvironment.GetGame().GetItemManager().GetItem(this.BaseItem, out ItemData itemData))
                {
                    this.Data = itemData;
                }
            }

            return this.Data;
        }

        public Room GetRoom()
        {
            return this._roomInstance;
        }

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
}
