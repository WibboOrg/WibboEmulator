using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Core;
using Butterfly.HabboHotel.Items.Interactors;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Map.Movement;
using Butterfly.HabboHotel.Rooms.Pathfinding;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Butterfly.HabboHotel.Items
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
        public Team Team;
        public int InteractionCountHelper;
        public int Value;
        public FreezePowerUp FreezePowerUp;
        public int Rotation;
        public string WallCoord;
        public int UpdateCounter;
        public int InteractingUser;
        public int InteractingUser2;
        private Room mRoom;
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

        public Dictionary<int, ThreeDCoord> GetAffectedTiles { get; private set; }

        public int GetX { get; private set; }

        public int GetY { get; private set; }

        public double GetZ { get; private set; }

        public bool IsRoller { get; private set; }

        public Point Coordinate => new Point(this.GetX, this.GetY);

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

        public double TotalHeight => this.GetZ + this.Height;

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
                Point point = new Point(this.GetX, this.GetY);
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
                Point point = new Point(this.GetX, this.GetY);
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
                    case InteractionType.banzaipyramid:
                        return new InteractorGate();
                    case InteractionType.SCOREBOARD:
                        return new InteractorScoreboard();
                    case InteractionType.VENDINGMACHINE:
                        return new InteractorVendor();
                    case InteractionType.vendingenablemachine:
                        return new InteractorVendorEnable();
                    case InteractionType.ALERT:
                        return new InteractorAlert();
                    case InteractionType.onewaygate:
                        return new InteractorOneWayGate();
                    case InteractionType.loveshuffler:
                        return new InteractorLoveShuffler();
                    case InteractionType.habbowheel:
                        return new InteractorHabboWheel();
                    case InteractionType.DICE:
                        return new InteractorDice();
                    case InteractionType.bottle:
                        return new InteractorSpinningBottle();
                    case InteractionType.TELEPORT:
                        return new InteractorTeleport();
                    case InteractionType.football:
                        return new InteractorFootball();
                    case InteractionType.footballcountergreen:
                    case InteractionType.footballcounteryellow:
                    case InteractionType.footballcounterblue:
                    case InteractionType.footballcounterred:
                        return new InteractorScoreCounter();
                    case InteractionType.banzaiscoreblue:
                    case InteractionType.banzaiscorered:
                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.banzaiscoregreen:
                        return new InteractorBanzaiScoreCounter();
                    case InteractionType.ChronoTimer:
                        return new InteractorTimer();
                    case InteractionType.banzaiblo:
                    case InteractionType.banzaiblob:
                        return new InteractorBlob();
                    case InteractionType.banzaipuck:
                        return new InteractorBanzaiPuck();
                    case InteractionType.freezetileblock:
                    case InteractionType.freezetile:
                        return new InteractorFreezeTile();
                    case InteractionType.jukebox:
                        return new InteractorJukebox();
                    case InteractionType.triggertimer:
                    case InteractionType.triggerroomenter:
                    case InteractionType.triggergameend:
                    case InteractionType.triggergamestart:
                    case InteractionType.triggerrepeater:
                    case InteractionType.triggerrepeaterlong:
                    case InteractionType.triggeronusersay:
                    case InteractionType.triggercommand:
                    case InteractionType.WIRED_TRIGGER_SELF:
                    case InteractionType.triggercollisionuser:
                    case InteractionType.triggerscoreachieved:
                    case InteractionType.triggerstatechanged:
                    case InteractionType.triggerwalkonfurni:
                    case InteractionType.triggerwalkofffurni:
                    case InteractionType.triggercollision:
                    case InteractionType.actiongivescore:
                    case InteractionType.actionposreset:
                    case InteractionType.actionmoverotate:
                    case InteractionType.actionresettimer:
                    case InteractionType.actionshowmessage:
                    case InteractionType.highscore:
                    case InteractionType.highscorepoints:
                    case InteractionType.actiongivereward:
                    case InteractionType.superwired:
                    case InteractionType.superwiredcondition:
                    case InteractionType.actionteleportto:
                    case InteractionType.wf_act_endgame_team:
                    case InteractionType.wf_act_call_stacks:
                    case InteractionType.actiontogglestate:
                    case InteractionType.actionkickuser:
                    case InteractionType.actionflee:
                    case InteractionType.actionchase:
                    case InteractionType.collisioncase:
                    case InteractionType.collisionteam:
                    case InteractionType.actionmovetodir:
                    case InteractionType.conditionfurnishaveusers:
                    case InteractionType.conditionfurnishavenousers:
                    case InteractionType.conditionhasfurnionfurni:
                    case InteractionType.conditionhasfurnionfurniNegative:
                    case InteractionType.conditionstatepos:
                    case InteractionType.wf_cnd_stuff_is:
                    case InteractionType.wf_cnd_not_stuff_is:
                    case InteractionType.conditionstateposNegative:
                    case InteractionType.conditiontimelessthan:
                    case InteractionType.conditiontimemorethan:
                    case InteractionType.conditiontriggeronfurni:
                    case InteractionType.conditiontriggeronfurniNegative:
                    case InteractionType.conditionactoringroup:
                    case InteractionType.conditionnotingroup:
                    case InteractionType.wf_trg_bot_reached_stf:
                    case InteractionType.wf_trg_bot_reached_avtr:
                    case InteractionType.wf_act_bot_clothes:
                    case InteractionType.wf_act_bot_teleport:
                    case InteractionType.wf_act_bot_follow_avatar:
                    case InteractionType.wf_act_bot_give_handitem:
                    case InteractionType.wf_act_bot_move:
                    case InteractionType.wf_act_user_move:
                    case InteractionType.wf_act_bot_talk_to_avatar:
                    case InteractionType.wf_act_bot_talk:
                    case InteractionType.wf_cnd_has_handitem:
                    case InteractionType.wf_act_join_team:
                    case InteractionType.wf_act_leave_team:
                    case InteractionType.wf_act_give_score_tm:
                    case InteractionType.wf_cnd_actor_in_team:
                    case InteractionType.wf_cnd_not_in_team:
                    case InteractionType.wf_cnd_not_user_count:
                    case InteractionType.wf_cnd_user_count_in:
                        return new WiredInteractor();
                    case InteractionType.MANNEQUIN:
                        return new InteractorManiqui();
                    case InteractionType.TONER:
                        return new InteractorChangeBackgrounds();
                    case InteractionType.puzzlebox:
                        return new InteractorPuzzleBox();
                    case InteractionType.floorswitch1:
                        return new InteractorSwitch1(this.GetBaseItem().Modes);
                    case InteractionType.CRACKABLE:
                        return new InteractorCrackable(this.GetBaseItem().Modes);
                    case InteractionType.tvyoutube:
                        return new InteractorTvYoutube();
                    case InteractionType.LOVELOCK:
                        return new InteractorLoveLock();
                    case InteractionType.PHOTO:
                        return new InteractorIgnore();
                    default:
                        return new InteractorGenericSwitch(this.GetBaseItem().Modes);
                }
            }
        }

        public Item(int mId, int RoomId, int mBaseItem, string ExtraData, int limitedNumber, int limitedStack, int X, int Y, double Z, int Rot, string wallCoord, Room pRoom)
        {
            if (ButterflyEnvironment.GetGame().GetItemManager().GetItem(mBaseItem, out ItemData Data))
            {
                this.Id = mId;
                this.RoomId = RoomId;
                this.BaseItem = mBaseItem;
                this.ExtraData = ExtraData;
                this.GetX = X;
                this.GetY = Y;
                if (!double.IsInfinity(Z))
                {
                    this.GetZ = Z;
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

                this.mRoom = pRoom;
                if (this.GetBaseItem() == null)
                {
                    Logging.LogException("Unknown baseID: " + mBaseItem);
                }

                switch (this.GetBaseItem().InteractionType)
                {
                    case InteractionType.roller:
                        this.IsRoller = true;
                        break;
                    case InteractionType.footballcountergreen:
                    case InteractionType.banzaigategreen:
                    case InteractionType.banzaiscoregreen:
                    case InteractionType.freezegreencounter:
                    case InteractionType.freezegreengate:
                        this.Team = Team.green;
                        break;
                    case InteractionType.footballcounteryellow:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.freezeyellowcounter:
                    case InteractionType.freezeyellowgate:
                        this.Team = Team.yellow;
                        break;
                    case InteractionType.footballcounterblue:
                    case InteractionType.banzaigateblue:
                    case InteractionType.banzaiscoreblue:
                    case InteractionType.freezebluecounter:
                    case InteractionType.freezebluegate:
                        this.Team = Team.blue;
                        break;
                    case InteractionType.footballcounterred:
                    case InteractionType.banzaigatered:
                    case InteractionType.banzaiscorered:
                    case InteractionType.freezeredcounter:
                    case InteractionType.freezeredgate:
                        this.Team = Team.red;
                        break;
                    case InteractionType.banzaitele:
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

                this.GetAffectedTiles = Gamemap.GetAffectedTiles(this.GetBaseItem().Length, this.GetBaseItem().Width, this.GetX, this.GetY, Rot);
            }
        }

        public void SetState(int pX, int pY, double pZ, Dictionary<int, ThreeDCoord> Tiles)
        {
            this.GetX = pX;
            this.GetY = pY;
            if (!double.IsInfinity(pZ))
            {
                this.GetZ = pZ;
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
            this.mRoom = null;
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
                        Y = Y - i;
                        break;
                    }
                case MovementDirection.upright:
                    {
                        X = X + i;
                        Y = Y - i;
                        break;
                    }
                case MovementDirection.right:
                    {
                        X = X + i;
                        break;
                    }
                case MovementDirection.downright:
                    {
                        X = X + i;
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.down:
                    {
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.downleft:
                    {
                        X = X - i;
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.left:
                    {
                        X = X - i;
                        break;
                    }
                case MovementDirection.upleft:
                    {
                        X = X - i;
                        Y = Y - i;
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

            switch (this.GetBaseItem().InteractionType)
            {
                case InteractionType.football:
                    if (this.InteractionCountHelper <= 0 || this.InteractionCountHelper > 6)
                    {
                        this.ExtraData = "0";
                        this.UpdateState(false, true);

                        this.InteractionCountHelper = 0;
                        break;
                    }

                    int Length = 1;
                    int OldX = this.GetX;
                    int OldY = this.GetY;

                    int NewX = this.GetX;
                    int NewY = this.GetY;

                    Point NewPoint = this.GetMoveCoord(OldX, OldY, 1);

                    if (this.InteractionCountHelper > 3)
                    {
                        Length = 3;

                        this.ExtraData = "6";
                        this.UpdateState(false, true);
                    }
                    else if (this.InteractionCountHelper > 1 && this.InteractionCountHelper < 4)
                    {
                        Length = 2;

                        this.ExtraData = "4";
                        this.UpdateState(false, true);
                    }
                    else
                    {
                        Length = 1;

                        this.ExtraData = "2";
                        this.UpdateState(false, true);
                    }


                    if (Length != 1 && !this.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
                    {
                        this.GetNewDir(NewX, NewY);
                        this.InteractionCountHelper--;
                    }

                    for (int i = 1; i <= Length; i++)
                    {
                        NewPoint = this.GetMoveCoord(OldX, OldY, i);


                        if ((this.InteractionCountHelper <= 3 && this.GetRoom().GetGameMap().SquareHasUsers(NewPoint.X, NewPoint.Y)))
                        {
                            this.InteractionCountHelper = 0;
                            break;
                        }

                        if (this.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
                        {
                            NewX = NewPoint.X;
                            NewY = NewPoint.Y;
                            this.GetRoom().GetSoccer().HandleFootballGameItems(new Point(NewPoint.X, NewPoint.Y));
                        }
                        else
                        {
                            this.GetNewDir(NewX, NewY);
                            this.InteractionCountHelper--;
                            break;
                        }

                        if (!this.GetRoom().GetGameMap().SquareTakingOpen(NewPoint.X, NewPoint.Y))
                        {
                            List<RoomUser> Users = this.GetRoom().GetGameMap().GetNearUsers(new Point(NewPoint.X, NewPoint.Y), 1);
                            if (Users != null)
                            {
                                bool BreakMe = false;
                                foreach (RoomUser User in Users)
                                {
                                    if (User == null || this.InteractingUser == User.VirtualId)
                                    {
                                        continue;
                                    }

                                    if (User.SetX != NewPoint.X || User.SetY != NewPoint.Y)
                                    {
                                        continue;
                                    }

                                    if (User.SetStep && User.SetX == User.GoalX && User.SetY == User.GoalY)
                                    {
                                        this.InteractionCountHelper = 6;
                                        this.InteractingUser = User.VirtualId;
                                        this.MovementDir = MovementManagement.GetMovementByDirection(User.RotBody);
                                        BreakMe = true;
                                        break;
                                    }
                                }

                                if (BreakMe)
                                {
                                    break;
                                }
                            }
                        }

                        this.InteractionCountHelper--;
                    }

                    double Z = this.GetRoom().GetGameMap().SqAbsoluteHeight(NewX, NewY);
                    this.GetRoom().GetRoomItemHandler().PositionReset(this, NewX, NewY, Z);

                    this.UpdateCounter = 1;
                    break;
                case InteractionType.ChronoTimer:
                    if (string.IsNullOrEmpty(this.ExtraData))
                    {
                        break;
                    }

                    int NumChrono = 0;
                    if (!int.TryParse(this.ExtraData, out NumChrono))
                    {
                        break;
                    }

                    if (!this.ChronoStarter)
                    {
                        break;
                    }

                    if (NumChrono > 0)
                    {
                        if (this.InteractionCountHelper == 1)
                        {
                            NumChrono--;
                            /*if (!this.GetRoom().GetBanzai().isBanzaiActive || !this.GetRoom().GetFreeze().isGameActive)
                            {
                                NumChrono = 0;
                            }*/
                            this.InteractionCountHelper = 0;
                            this.ExtraData = NumChrono.ToString();
                            this.UpdateState();
                        }
                        else
                        {
                            this.InteractionCountHelper++;
                        }

                        this.UpdateCounter = 1;
                        break;
                    }
                    else
                    {
                        this.ChronoStarter = false;
                        this.GetRoom().GetGameManager().StopGame();
                        break;
                    }
                case InteractionType.banzaitele:
                    if (this.InteractingUser == 0)
                    {
                        this.ExtraData = string.Empty;
                        this.UpdateState();
                        break;
                    }

                    this.ExtraData = "1";
                    this.UpdateState();

                    this.UpdateCounter = 1;

                    RoomUser roomUserByHabbo = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(this.InteractingUser);
                    if (roomUserByHabbo != null)
                    {
                        this.GetRoom().GetGameMap().TeleportToItem(roomUserByHabbo, this);
                        roomUserByHabbo.SetRot(ButterflyEnvironment.GetRandomNumber(0, 7), false);
                        roomUserByHabbo.CanWalk = true;
                    }
                    this.InteractingUser = 0;

                    break;
                case InteractionType.freezetile:
                    if (this.InteractingUser <= 0)
                    {
                        break;
                    }

                    RoomUser roomUserByHabbo3 = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(this.InteractingUser);
                    if (roomUserByHabbo3 != null)
                    {
                        roomUserByHabbo3.CountFreezeBall = 1;
                    }
                    this.ExtraData = "11000";
                    this.UpdateState(false, true);
                    this.GetRoom().GetFreeze().onFreezeTiles(this, this.FreezePowerUp, this.InteractingUser);
                    this.InteractingUser = 0;
                    this.InteractionCountHelper = 0;
                    break;
                case InteractionType.SCOREBOARD:
                    if (string.IsNullOrEmpty(this.ExtraData))
                    {
                        break;
                    }

                    int num4 = 0;
                    try
                    {
                        num4 = int.Parse(this.ExtraData);
                    }
                    catch
                    {
                    }
                    if (num4 > 0)
                    {
                        if (this.InteractionCountHelper == 1)
                        {
                            int num2 = num4 - 1;
                            this.InteractionCountHelper = 0;
                            this.ExtraData = num2.ToString();
                            this.UpdateState();
                        }
                        else
                        {
                            this.InteractionCountHelper++;
                        }

                        this.UpdateCounter = 1;
                        break;
                    }
                    else
                    {
                        this.UpdateCounter = 0;
                        break;
                    }
                case InteractionType.VENDINGMACHINE:
                    if (!(this.ExtraData == "1"))
                    {
                        break;
                    }

                    RoomUser roomUserByHabbo1 = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(this.InteractingUser);
                    if (roomUserByHabbo1 != null)
                    {
                        int num2 = this.GetBaseItem().VendingIds[ButterflyEnvironment.GetRandomNumber(0, this.GetBaseItem().VendingIds.Count - 1)];
                        roomUserByHabbo1.CarryItem(num2);
                    }
                    this.InteractingUser = 0;
                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                    break;
                case InteractionType.vendingenablemachine:
                    if (!(this.ExtraData == "1"))
                    {
                        break;
                    }

                    RoomUser roomUserByHabboEnable = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(this.InteractingUser);
                    if (roomUserByHabboEnable != null)
                    {
                        int num2 = this.GetBaseItem().VendingIds[ButterflyEnvironment.GetRandomNumber(0, this.GetBaseItem().VendingIds.Count - 1)];
                        roomUserByHabboEnable.ApplyEffect(num2);
                    }
                    this.InteractingUser = 0;
                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                    break;
                case InteractionType.ALERT:
                    if (!(this.ExtraData == "1"))
                    {
                        break;
                    }

                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                    break;
                case InteractionType.onewaygate:
                    RoomUser roomUser3 = null;
                    if (this.InteractingUser > 0)
                    {
                        roomUser3 = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(this.InteractingUser);
                    }

                    if (roomUser3 == null)
                    {
                        this.InteractingUser = 0;
                        break;
                    }

                    if (roomUser3.Coordinate == this.SquareBehind || !Gamemap.TilesTouching(this.GetX, this.GetY, roomUser3.X, roomUser3.Y))
                    {
                        roomUser3.UnlockWalking();
                        this.ExtraData = "0";
                        this.InteractingUser = 0;
                        this.UpdateState(false, true);
                    }
                    else
                    {
                        roomUser3.CanWalk = false;
                        roomUser3.AllowOverride = true;
                        roomUser3.MoveTo(this.SquareBehind);

                        this.UpdateCounter = 1;
                    }

                    break;
                case InteractionType.loveshuffler:
                    if (this.ExtraData == "0")
                    {
                        this.ExtraData = ButterflyEnvironment.GetRandomNumber(1, 4).ToString();
                        this.ReqUpdate(20);
                    }
                    else if (this.ExtraData != "-1")
                    {
                        this.ExtraData = "-1";
                    }

                    this.UpdateState(false, true);
                    break;
                case InteractionType.habbowheel:
                    this.ExtraData = ButterflyEnvironment.GetRandomNumber(1, 10).ToString();
                    this.UpdateState();
                    break;
                case InteractionType.DICE:
                    this.ExtraData = ButterflyEnvironment.GetRandomNumber(1, 6).ToString();
                    this.UpdateState();
                    break;
                case InteractionType.bottle:
                    this.ExtraData = ButterflyEnvironment.GetRandomNumber(0, 7).ToString();
                    this.UpdateState();
                    break;
                case InteractionType.TELEPORT:
                case InteractionType.ARROW:
                    bool keepDoorOpen = false;
                    bool showTeleEffect = false;
                    if (this.InteractingUser > 0)
                    {
                        RoomUser roomUserByHabbo2 = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(this.InteractingUser);
                        if (roomUserByHabbo2 != null)
                        {
                            if (roomUserByHabbo2.Coordinate == this.Coordinate)
                            {
                                roomUserByHabbo2.AllowOverride = false;
                                if (ItemTeleporterFinder.IsTeleLinked(this.Id, this.mRoom))
                                {
                                    showTeleEffect = true;
                                    int linkedTele = ItemTeleporterFinder.GetLinkedTele(this.Id);
                                    int teleRoomId = ItemTeleporterFinder.GetTeleRoomId(linkedTele, this.mRoom);
                                    if (teleRoomId == this.RoomId)
                                    {
                                        Item roomItem = this.GetRoom().GetRoomItemHandler().GetItem(linkedTele);
                                        if (roomItem == null)
                                        {
                                            roomUserByHabbo2.UnlockWalking();
                                        }
                                        else
                                        {
                                            roomUserByHabbo2.SetRot(roomItem.Rotation, false);
                                            roomItem.GetRoom().GetGameMap().TeleportToItem(roomUserByHabbo2, roomItem);
                                            roomItem.ExtraData = "2";
                                            roomItem.UpdateState(false, true);
                                            roomItem.InteractingUser2 = this.InteractingUser;
                                            roomItem.ReqUpdate(2);
                                        }
                                    }
                                    else if (!roomUserByHabbo2.IsBot && roomUserByHabbo2 != null && (roomUserByHabbo2.GetClient() != null && roomUserByHabbo2.GetClient().GetHabbo() != null))
                                    {
                                        roomUserByHabbo2.GetClient().GetHabbo().IsTeleporting = true;
                                        roomUserByHabbo2.GetClient().GetHabbo().TeleportingRoomID = teleRoomId;
                                        roomUserByHabbo2.GetClient().GetHabbo().TeleporterId = linkedTele;
                                        roomUserByHabbo2.GetClient().GetHabbo().PrepareRoom(teleRoomId, "");
                                    }
                                    this.InteractingUser = 0;
                                }
                                else
                                {
                                    roomUserByHabbo2.UnlockWalking();
                                    this.InteractingUser = 0;
                                }
                            }
                            else if (roomUserByHabbo2.Coordinate == this.SquareInFront)
                            {
                                roomUserByHabbo2.AllowOverride = true;
                                keepDoorOpen = true;

                                roomUserByHabbo2.CanWalk = false;
                                roomUserByHabbo2.AllowOverride = true;
                                roomUserByHabbo2.MoveTo(this.Coordinate.X, this.Coordinate.Y, true);
                            }
                            else
                            {
                                this.InteractingUser = 0;
                            }
                        }
                        else
                        {
                            this.InteractingUser = 0;
                        }

                        this.UpdateCounter = 1;
                    }
                    if (this.InteractingUser2 > 0)
                    {
                        RoomUser roomUserByHabbo2 = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(this.InteractingUser2);
                        if (roomUserByHabbo2 != null)
                        {
                            keepDoorOpen = true;
                            roomUserByHabbo2.UnlockWalking();
                            roomUserByHabbo2.MoveTo(this.SquareInFront);
                        }
                        this.UpdateCounter = 1;
                        this.InteractingUser2 = 0;
                    }
                    if (keepDoorOpen)
                    {
                        if (this.ExtraData != "1")
                        {
                            this.ExtraData = "1";
                            this.UpdateState(false, true);
                        }
                    }
                    else if (showTeleEffect)
                    {
                        if (this.ExtraData != "2")
                        {
                            this.ExtraData = "2";
                            this.UpdateState(false, true);
                        }
                    }
                    else if (this.ExtraData != "0")
                    {
                        this.ExtraData = "0";
                        this.UpdateState(false, true);
                    }

                    break;
            }
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
                case InteractionType.roller:
                    this.IsRoller = true;
                    break;
                case InteractionType.footballcountergreen:
                case InteractionType.banzaigategreen:
                case InteractionType.banzaiscoregreen:
                case InteractionType.freezegreencounter:
                case InteractionType.freezegreengate:
                    this.Team = Team.green;
                    break;
                case InteractionType.footballcounteryellow:
                case InteractionType.banzaigateyellow:
                case InteractionType.banzaiscoreyellow:
                case InteractionType.freezeyellowcounter:
                case InteractionType.freezeyellowgate:
                    this.Team = Team.yellow;
                    break;
                case InteractionType.footballcounterblue:
                case InteractionType.banzaigateblue:
                case InteractionType.banzaiscoreblue:
                case InteractionType.freezebluecounter:
                case InteractionType.freezebluegate:
                    this.Team = Team.blue;
                    break;
                case InteractionType.footballcounterred:
                case InteractionType.banzaigatered:
                case InteractionType.banzaiscorered:
                case InteractionType.freezeredcounter:
                case InteractionType.freezeredgate:
                    this.Team = Team.red;
                    break;
                case InteractionType.banzaitele:
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

            this.GetAffectedTiles = Gamemap.GetAffectedTiles(this.GetBaseItem().Length, this.GetBaseItem().Width, this.GetX, this.GetY, this.Rotation);
        }

        public ItemData GetBaseItem()
        {
            if (this.Data == null)
            {
                if (ButterflyEnvironment.GetGame().GetItemManager().GetItem(this.BaseItem, out ItemData I))
                {
                    this.Data = I;
                }
            }

            return this.Data;
        }

        public Room GetRoom()
        {
            if (this.mRoom == null)
            {
                this.mRoom = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
            }

            return this.mRoom;
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
