using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using Butterfly.Game.Pets;
using Butterfly.Game.Roleplay;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms.AI;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Butterfly.Game.Rooms
{
    public class RoomUser : IEquatable<RoomUser>
    {
        public int UserId;
        public int HabboId;
        public int VirtualId;
        public int RoomId;
        public int IdleTime;

        public int X;
        public int Y;
        public double Z;
        public int GoalX;
        public int GoalY;

        public bool SetStep;
        public int SetX;
        public int SetY;
        public double SetZ;

        public int CarryItemID;
        public int CarryTimer;
        public int RotHead;
        public int RotBody;
        public bool CanWalk;
        public bool AllowOverride;
        public bool TeleportEnabled;

        public bool AllowMoveToRoller;

        private GameClient Client;
        public RoomBot BotData;
        public Pet PetData;
        public BotAI BotAI;
        public Room Room;

        public ItemEffectType CurrentItemEffect;
        public bool Freezed;
        public int FreezeCounter;
        public Team Team;
        public FreezePowerUp BanzaiPowerUp;
        public int FreezeLives;
        public bool ShieldActive;
        public int ShieldCounter;
        public int CountFreezeBall = 1;
        public bool MoonwalkEnabled;
        public bool FacewalkEnabled;
        public bool RidingHorse;
        public bool IsSit;
        public bool IsLay;
        public int HorseID;
        public bool IsWalking;
        public bool UpdateNeeded;
        public bool IsAsleep;
        public Dictionary<string, string> Statusses;
        public int DanceId;
        public int FloodCount;
        public bool IsSpectator;

        public bool ConstruitEnable = false;
        public bool ConstruitZMode = false;
        public double ConstruitHeigth = 1.0;

        public bool Freeze;
        public int FreezeEndCounter;
        public bool transformation;
        public bool transfbot;
        public string transformationrace;

        public bool AllowBall;
        public bool MoveWithBall;
        public bool SetMoveWithBall;
        public bool AllowShoot;

        public string ChatTextColor;

        public string LastMessage;
        public int LastMessageCount;

        public int PartyId;
        public int TimerResetEffect;

        public string LoaderVideoId;

        public int WiredPoints;
        public bool InGame;
        public bool WiredGivelot;

        //Walk
        public bool BreakWalkEnable;
        public bool StopWalking;
        public bool ReverseWalk;
        public bool WalkSpeed;
        public bool AllowMoveTo;

        public int LLPartner = 0;

        public int CurrentEffect;

        //RP STATS TEMP
        public List<int> AllowBuyItems;

        public bool IsDispose;
        public int UserTimer;

        public List<string> WhiperGroupUsers;
        public bool muted;


        public Point Coordinate => new Point(this.X, this.Y);

        public bool IsPet
        {
            get
            {
                if (this.IsBot)
                {
                    return this.BotData.IsPet;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsDancing => this.DanceId >= 1;

        public bool NeedsAutokick => !this.IsBot && (this.GetClient() == null || this.GetClient().GetHabbo() == null || this.GetClient().GetHabbo().Rank < 2 && this.IdleTime >= 1200);

        public bool IsTrading => !this.IsBot && this.Statusses.ContainsKey("/trd");

        public bool IsBot => this.BotData != null;

        public RolePlayer Roleplayer
        {
            get
            {
                RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.GetRoom().RoomData.OwnerId);
                if (RPManager == null)
                {
                    return null;
                }

                RolePlayer Rp = RPManager.GetPlayer(this.UserId);
                if (Rp == null)
                {
                    return null;
                }

                return Rp;
            }
        }

        public RoomUser(int HabboId, int RoomId, int VirtualId, Room room)
        {
            this.Freezed = false;
            this.HabboId = HabboId;
            this.RoomId = RoomId;
            this.VirtualId = VirtualId;
            this.IdleTime = 0;
            this.X = 0;
            this.Y = 0;
            this.Z = 0.0;
            this.RotHead = 0;
            this.RotBody = 0;
            this.UpdateNeeded = true;
            this.Statusses = new Dictionary<string, string>();
            this.Room = room;
            this.AllowOverride = false;
            this.CanWalk = true;
            this.CurrentItemEffect = ItemEffectType.NONE;
            this.BreakWalkEnable = false;
            this.AllowShoot = false;
            this.AllowBuyItems = new List<int>();
            this.IsDispose = false;
            this.AllowMoveTo = true;
            this.WhiperGroupUsers = new List<string>();
        }

        public bool Equals(RoomUser comparedUser)
        {
            if (comparedUser == null)
            {
                return false;
            }

            //if (comparedUser.HabboId > 0 || this.HabboId > 0)
            //return comparedUser.HabboId == this.HabboId;

            return comparedUser.VirtualId == this.VirtualId;
        }

        public string GetUsername()
        {
            if (this.IsBot)
            {
                return this.BotData.Name;
            }
            else if (this.IsPet)
            {
                return this.PetData.Name;
            }
            else if (this.GetClient() != null && this.GetClient().GetHabbo() != null)
            {
                return this.GetClient().GetHabbo().Username;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool IsOwner()
        {
            if (this.IsBot)
            {
                return false;
            }
            else
            {
                return this.GetUsername() == this.GetRoom().RoomData.OwnerName;
            }
        }

        public void Unidle()
        {
            this.IdleTime = 0;
            if (!this.IsAsleep)
            {
                return;
            }

            this.IsAsleep = false;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_IDLE);
            Message.WriteInteger(this.VirtualId);
            Message.WriteBoolean(false);
            this.GetRoom().SendPacket(Message);
        }

        public void Dispose()
        {
            this.Statusses.Clear();
            this.IsDispose = true;
            this.Room = null;
            this.Client = null;
        }

        public void SendWhisperChat(string message, bool Info = true)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
            Message.WriteInteger(this.VirtualId);
            Message.WriteString(message);
            Message.WriteInteger(0);
            Message.WriteInteger((Info) ? 34 : 0);
            Message.WriteInteger(0);
            Message.WriteInteger(-1);
            this.GetClient().SendPacket(Message);
        }

        public void OnChatMe(string MessageText, int Color = 0, bool Shout = false)
        {
            int Header = ServerPacketHeader.UNIT_CHAT;
            if (Shout)
            {
                Header = ServerPacketHeader.UNIT_CHAT_SHOUT;
            }

            ServerPacket Message = new ServerPacket(Header);
            Message.WriteInteger(this.VirtualId);
            Message.WriteString(MessageText);
            Message.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(MessageText));
            Message.WriteInteger(Color);
            Message.WriteInteger(0);
            Message.WriteInteger(-1);
            this.GetClient().SendPacket(Message);
        }

        public void OnChat(string MessageText, int Color = 0, bool Shout = false)
        {
            int Header = ServerPacketHeader.UNIT_CHAT;
            if (Shout)
            {
                Header = ServerPacketHeader.UNIT_CHAT_SHOUT;
            }

            ServerPacket Message = new ServerPacket(Header);
            Message.WriteInteger(this.VirtualId);
            Message.WriteString(MessageText);
            Message.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(MessageText));
            Message.WriteInteger(Color);
            Message.WriteInteger(0);
            Message.WriteInteger(-1);
            this.GetRoom().SendPacketOnChat(Message, this, true, (this.Team == Team.none && !this.IsBot));
        }

        public void MoveTo(Point c, bool Override = false)
        {
            this.MoveTo(c.X, c.Y, Override);
        }

        public void MoveTo(int pX, int pY, bool pOverride = false)
        {
            if (!this.GetRoom().GetGameMap().CanWalkState(pX, pY, pOverride) || this.Freeze || !this.AllowMoveTo)
            {
                return;
            }

            this.Unidle();
            if (this.TeleportEnabled)
            {
                this.GetRoom().SendPacket(this.GetRoom().GetRoomItemHandler().TeleportUser(this, new Point(pX, pY), 0, this.GetRoom().GetGameMap().SqAbsoluteHeight(pX, pY)));
            }
            else
            {
                this.IsWalking = true;
                this.GoalX = pX;
                this.GoalY = pY;
            }
        }

        public void UnlockWalking()
        {
            this.AllowOverride = false;
            this.CanWalk = true;
        }

        public void SetPosRoller(int pX, int pY, double pZ)
        {
            this.SetX = pX;
            this.SetY = pY;
            this.SetZ = pZ;
            this.SetStep = true;

            this.GetRoom().GetGameMap().AddTakingSquare(pX, pY);

            this.UpdateNeeded = false;
            this.IsWalking = false;
        }

        public void SetPos(int pX, int pY, double pZ)
        {
            this.GetRoom().GetGameMap().UpdateUserMovement(this.Coordinate, new Point(pX, pY), this);
            this.X = pX;
            this.Y = pY;
            this.Z = pZ;

            this.SetX = pX;
            this.SetY = pY;
            this.SetZ = pZ;

            this.GoalX = this.X;
            this.GoalY = this.Y;
            this.SetStep = false;
            this.IsWalking = false;
            this.UpdateNeeded = true;
        }

        public void CarryItem(int Item, bool notTimer = false)
        {
            this.CarryItemID = Item;
            this.CarryTimer = Item <= 0 || notTimer ? 0 : 240;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_HAND_ITEM);
            Message.WriteInteger(this.VirtualId);
            Message.WriteInteger(Item);
            this.GetRoom().SendPacket(Message);
        }

        public void SetRot(int Rotation, bool HeadOnly, bool IgnoreWalk = false)
        {
            if (this.Statusses.ContainsKey("lay") || (this.IsWalking && !IgnoreWalk))
            {
                return;
            }

            int num = this.RotBody - Rotation;
            this.RotHead = this.RotBody;
            if (HeadOnly || this.Statusses.ContainsKey("sit"))
            {
                if (this.RotBody == 2 || this.RotBody == 4)
                {
                    if (num > 0)
                    {
                        this.RotHead = this.RotBody - 1;
                    }
                    else if (num < 0)
                    {
                        this.RotHead = this.RotBody + 1;
                    }
                }
                else if (this.RotBody == 0 || this.RotBody == 6)
                {
                    if (num > 0)
                    {
                        this.RotHead = this.RotBody - 1;
                    }
                    else if (num < 0)
                    {
                        this.RotHead = this.RotBody + 1;
                    }
                }
            }
            else if (num <= -2 || num >= 2)
            {
                this.RotHead = Rotation;
                this.RotBody = Rotation;
            }
            else
            {
                this.RotHead = Rotation;
            }

            this.UpdateNeeded = true;
        }

        public void SetStatus(string Key, string Value)
        {
            if (this.Statusses.ContainsKey(Key))
            {
                this.Statusses[Key] = Value;
            }
            else
            {
                this.Statusses.Add(Key, Value);
            }
        }

        public void RemoveStatus(string Key)
        {
            if (!this.Statusses.ContainsKey(Key))
            {
                return;
            }

            this.Statusses.Remove(Key);
        }

        public void ApplyEffect(int EffectId, bool DontSave = false)
        {
            if (this.Room == null)
            {
                return;
            }

            if (this.RidingHorse && (EffectId != 77 && EffectId != 103))
            {
                return;
            }

            if (this.CurrentEffect == EffectId && !DontSave)
            {
                return;
            }

            if (!DontSave)
            {
                this.CurrentEffect = EffectId;
            }

            ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_EFFECT);
            Message.WriteInteger(this.VirtualId);
            Message.WriteInteger(EffectId);
            Message.WriteInteger(2);
            this.Room.SendPacket(Message);
        }

        public bool SetPetTransformation(string NamePet, int RaceId)
        {
            string RaceData = TransformUtility.GetRace(NamePet, RaceId);

            if (string.IsNullOrEmpty(RaceData))
            {
                return false;
            }
            else
            {
                this.transformationrace = RaceData;

                return true;
            }
        }

        public GameClient GetClient()
        {
            if (this.IsBot)
            {
                return null;
            }

            if (this.Client == null)
            {
                this.Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.HabboId);
            }

            return this.Client;
        }

        private Room GetRoom()
        {
            if (this.Room == null)
            {
                this.Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
            }

            return this.Room;
        }
    }
}
