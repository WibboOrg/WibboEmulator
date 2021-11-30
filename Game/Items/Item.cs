using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Core;
using Butterfly.Game.Items.Interactors;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Rooms.Pathfinding;
using Butterfly.Game.Items.Wired.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using Butterfly.Utility.Events;

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
        public bool PendingReset;
        public int Fx;

        public bool ChronoStarter;
        public MovementState movement;
        public MovementDirection MovementDir;

        public Dictionary<string, int> Scores;

        public IWired WiredHandler;
        public event OnItemTrigger ItemTriggerEventHandler;
        public event UserAndItemDelegate OnUserWalksOffFurni;
        public event UserAndItemDelegate OnUserWalksOnFurni;

        private Room _roomInstance;

        public Dictionary<int, ThreeDCoord> GetAffectedTiles { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public double Z { get; private set; }

        public bool IsRoller { get; private set; }

        public Point Coordinate => new Point(this.X, this.Y);

        public List<Point> GetCoords
        {
            get
            {
                List<Point> list = new List<Point>
                {
                    this.Coordinate
                };
                foreach (ThreeDCoord threeDcoord in this.GetAffectedTiles.Values)
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
                    --point.Y;
                }
                else if (this.Rotation == 2)
                {
                    ++point.X;
                }
                else if (this.Rotation == 4)
                {
                    ++point.Y;
                }
                else if (this.Rotation == 6)
                {
                    --point.X;
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
                    ++point.Y;
                }
                else if (this.Rotation == 2)
                {
                    --point.X;
                }
                else if (this.Rotation == 4)
                {
                    --point.Y;
                }
                else if (this.Rotation == 6)
                {
                    ++point.X;
                }

                return point;
            }
        }

        public ItemData Data;

        public FurniInteractor Interactor
        {
            get
            {
                switch (this.GetBaseItem().InteractionType)
                {
                    case InteractionType.GATE:
                    case InteractionType.BANZAIPYRAMID:
                        return new InteractorGate();
                    case InteractionType.SCOREBOARD:
                        return new InteractorScoreboard();
                    case InteractionType.VENDINGMACHINE:
                        return new InteractorVendor();
                    case InteractionType.VENDINGENABLEMACHINE:
                        return new InteractorVendorEnable();
                    case InteractionType.ALERT:
                        return new InteractorAlert();
                    case InteractionType.ONEWAYGATE:
                        return new InteractorOneWayGate();
                    case InteractionType.LOVESHUFFLER:
                        return new InteractorLoveShuffler();
                    case InteractionType.HABBOWHEEL:
                        return new InteractorHabboWheel();
                    case InteractionType.DICE:
                        return new InteractorDice();
                    case InteractionType.BOTTLE:
                        return new InteractorSpinningBottle();
                    case InteractionType.ARROW:
                    case InteractionType.TELEPORT:
                        return new InteractorTeleport();
                    case InteractionType.FOOTBALL:
                        return new InteractorFootball();
                    case InteractionType.FOOTBALLCOUNTERGREEN:
                    case InteractionType.FOOTBALLCOUNTERYELLOW:
                    case InteractionType.FOOTBALLCOUNTERBLUE:
                    case InteractionType.FOOTBALLCOUNTERRED:
                        return new InteractorScoreCounter();
                    case InteractionType.BANZAISCOREBLUE:
                    case InteractionType.BANZAISCORERED:
                    case InteractionType.BANZAISCOREYELLOW:
                    case InteractionType.BANZAISCOREGREEN:
                        return new InteractorBanzaiScoreCounter();
                    case InteractionType.CHRONOTIMER:
                        return new InteractorTimer();
                    case InteractionType.BANZAIBLO:
                    case InteractionType.BANZAIBLOB:
                        return new InteractorBlob();
                    case InteractionType.BANZAIPUCK:
                        return new InteractorBanzaiPuck();
                    case InteractionType.FREEZETILEBLOCK:
                        return new InteractorFreezeBlock();
                    case InteractionType.FREEZETILE:
                        return new InteractorFreezeTile();
                    case InteractionType.JUKEBOX:
                        return new InteractorJukebox();
                    case InteractionType.TRIGGER_ONCE:
                    case InteractionType.TRIGGER_AVATAR_ENTERS_ROOM:
                    case InteractionType.TRIGGER_GAME_ENDS:
                    case InteractionType.TRIGGER_GAME_STARTS:
                    case InteractionType.TRIGGER_PERIODICALLY:
                    case InteractionType.TRIGGER_PERIODICALLY_LONG:
                    case InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING:
                    case InteractionType.TRIGGER_COMMAND:
                    case InteractionType.TRIGGER_SELF:
                    case InteractionType.TRIGGER_COLLISION_USER:
                    case InteractionType.TRIGGER_SCORE_ACHIEVED:
                    case InteractionType.TRIGGER_STATE_CHANGED:
                    case InteractionType.TRIGGER_WALK_ON_FURNI:
                    case InteractionType.TRIGGER_WALK_OFF_FURNI:
                    case InteractionType.TRIGGER_COLLISION:
                    case InteractionType.ACTION_GIVE_SCORE:
                    case InteractionType.ACTION_POS_RESET:
                    case InteractionType.ACTION_MOVE_ROTATE:
                    case InteractionType.ACTION_RESET_TIMER:
                    case InteractionType.ACTIONSHOWMESSAGE:
                    case InteractionType.HIGHSCORE:
                    case InteractionType.HIGHSCOREPOINTS:
                    case InteractionType.ACTION_GIVE_REWARD:
                    case InteractionType.ACTION_SUPER_WIRED:
                    case InteractionType.CONDITION_SUPER_WIRED:
                    case InteractionType.ACTION_TELEPORT_TO:
                    case InteractionType.ACTION_ENDGAME_TEAM:
                    case InteractionType.ACTION_CALL_STACKS:
                    case InteractionType.ACTION_TOGGLE_STATE:
                    case InteractionType.ACTION_KICK_USER:
                    case InteractionType.ACTION_FLEE:
                    case InteractionType.ACTION_CHASE:
                    case InteractionType.ACTION_COLLISION_CASE:
                    case InteractionType.ACTION_COLLISION_TEAM:
                    case InteractionType.ACTION_MOVE_TO_DIR:
                    case InteractionType.CONDITION_FURNIS_HAVE_USERS:
                    case InteractionType.CONDITION_FURNIS_HAVE_NO_USERS:
                    case InteractionType.CONDITION_HAS_FURNI_ON_FURNI:
                    case InteractionType.CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE:
                    case InteractionType.CONDITION_STATE_POS:
                    case InteractionType.CONDITION_STUFF_IS:
                    case InteractionType.CONDITION_NOT_STUFF_IS:
                    case InteractionType.CONDITION_DATE_RNG_ACTIVE:
                    case InteractionType.CONDITION_STATE_POS_NEGATIVE:
                    case InteractionType.CONDITION_TIME_LESS_THAN:
                    case InteractionType.CONDITION_TIME_MORE_THAN:
                    case InteractionType.CONDITION_TRIGGER_ON_FURNI:
                    case InteractionType.CONDITION_TRIGGER_ON_FURNI_NEGATIVE:
                    case InteractionType.CONDITION_ACTOR_IN_GROUP:
                    case InteractionType.CONDITION_NOT_IN_GROUP:
                    case InteractionType.TRIGGER_BOT_REACHED_STF:
                    case InteractionType.TRIGGER_BOT_REACHED_AVTR:
                    case InteractionType.ACTION_BOT_CLOTHES:
                    case InteractionType.ACTION_BOT_TELEPORT:
                    case InteractionType.ACTION_BOT_FOLLOW_AVATAR:
                    case InteractionType.ACTION_BOT_GIVE_HANDITEM:
                    case InteractionType.ACTION_BOT_MOVE:
                    case InteractionType.ACTION_USER_MOVE:
                    case InteractionType.ACTION_BOT_TALK_TO_AVATAR:
                    case InteractionType.ACTION_BOT_TALK:
                    case InteractionType.CONDITION_HAS_HANDITEM:
                    case InteractionType.ACTION_JOIN_TEAM:
                    case InteractionType.ACTION_LEAVE_TEAM:
                    case InteractionType.ACTION_GIVE_SCORE_TM:
                    case InteractionType.CONDITION_ACTOR_IN_TEAM:
                    case InteractionType.CONDITION_NOT_IN_TEAM:
                    case InteractionType.CONDITION_NOT_USER_COUNT:
                    case InteractionType.CONDITION_USER_COUNT_IN:
                        return new InteractorWired();
                    case InteractionType.MANNEQUIN:
                        return new InteractorManiqui();
                    case InteractionType.TONER:
                        return new InteractorChangeBackgrounds();
                    case InteractionType.PUZZLEBOX:
                        return new InteractorPuzzleBox();
                    case InteractionType.FLOORSWITCH1:
                        return new InteractorSwitch1(this.GetBaseItem().Modes);
                    case InteractionType.CRACKABLE:
                        return new InteractorCrackable(this.GetBaseItem().Modes);
                    case InteractionType.TVYOUTUBE:
                        return new InteractorTvYoutube();
                    case InteractionType.LOVELOCK:
                        return new InteractorLoveLock();
                    case InteractionType.PHOTO:
                        return new InteractorIgnore();
                    case InteractionType.BANZAITELE:
                        return new InteractorBanzaiTele();
                    default:
                        return new InteractorGenericSwitch(this.GetBaseItem().Modes);
                }
            }
        }

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

                this.Fx = this.Data.EffectId;

                this._roomInstance = room;
                if (this.GetBaseItem() == null)
                {
                    Logging.LogException("Unknown baseID: " + mBaseItem);
                }

                switch (this.GetBaseItem().InteractionType)
                {
                    case InteractionType.ROLLER:
                        this.IsRoller = true;
                        break;
                    case InteractionType.FOOTBALLCOUNTERGREEN:
                    case InteractionType.BANZAIGATEGREEN:
                    case InteractionType.BANZAISCOREGREEN:
                    case InteractionType.FREEZEGREENCOUNTER:
                    case InteractionType.FREEZEGREENGATE:
                        this.Team = TeamType.green;
                        break;
                    case InteractionType.FOOTBALLCOUNTERYELLOW:
                    case InteractionType.BANZAIGATEYELLOW:
                    case InteractionType.BANZAISCOREYELLOW:
                    case InteractionType.FREEZEYELLOWCOUNTER:
                    case InteractionType.FREEZEYELLOWGATE:
                        this.Team = TeamType.yellow;
                        break;
                    case InteractionType.FOOTBALLCOUNTERBLUE:
                    case InteractionType.BANZAIGATEBLUE:
                    case InteractionType.BANZAISCOREBLUE:
                    case InteractionType.FREEZEBLUECOUNTER:
                    case InteractionType.FREEZEBLUEGATE:
                        this.Team = TeamType.blue;
                        break;
                    case InteractionType.FOOTBALLCOUNTERRED:
                    case InteractionType.BANZAIGATERED:
                    case InteractionType.BANZAISCORERED:
                    case InteractionType.FREEZEREDCOUNTER:
                    case InteractionType.FREEZEREDGATE:
                        this.Team = TeamType.red;
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
            }
        }

        public void SetState(int pX, int pY, double pZ, Dictionary<int, ThreeDCoord> Tiles)
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
            this._roomInstance = null;
            this.GetAffectedTiles.Clear();

            if (this.WiredHandler != null)
            {
                this.WiredHandler.Dispose();
            }

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
                case InteractionType.ROLLER:
                    this.IsRoller = true;
                    break;
                case InteractionType.FOOTBALLCOUNTERGREEN:
                case InteractionType.BANZAIGATEGREEN:
                case InteractionType.BANZAISCOREGREEN:
                case InteractionType.FREEZEGREENCOUNTER:
                case InteractionType.FREEZEGREENGATE:
                    this.Team = TeamType.green;
                    break;
                case InteractionType.FOOTBALLCOUNTERYELLOW:
                case InteractionType.BANZAIGATEYELLOW:
                case InteractionType.BANZAISCOREYELLOW:
                case InteractionType.FREEZEYELLOWCOUNTER:
                case InteractionType.FREEZEYELLOWGATE:
                    this.Team = TeamType.yellow;
                    break;
                case InteractionType.FOOTBALLCOUNTERBLUE:
                case InteractionType.BANZAIGATEBLUE:
                case InteractionType.BANZAISCOREBLUE:
                case InteractionType.FREEZEBLUECOUNTER:
                case InteractionType.FREEZEBLUEGATE:
                    this.Team = TeamType.blue;
                    break;
                case InteractionType.FOOTBALLCOUNTERRED:
                case InteractionType.BANZAIGATERED:
                case InteractionType.BANZAISCORERED:
                case InteractionType.FREEZEREDCOUNTER:
                case InteractionType.FREEZEREDGATE:
                    this.Team = TeamType.red;
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
