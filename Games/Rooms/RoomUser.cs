namespace WibboEmulator.Games.Rooms;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplay.Player;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Rooms.Games.Freeze;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Utils;

public class RoomUser : IEquatable<RoomUser>
{
    public int UserId;
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

    public int LastBubble = 0;
    public int CarryItemID;
    public int CarryTimer;
    public int RotHead;
    public int RotBody;
    public bool CanWalk;
    public bool AllowOverride;
    public bool TeleportEnabled;

    public bool AllowMoveToRoller;

    private GameClient _client;

    public RoomBot BotData;
    public Pet PetData;
    public BotAI BotAI;
    public Room Room;

    public ItemEffectType CurrentItemEffect;
    public bool Freezed;
    public int FreezeCounter;
    public TeamType Team;
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
    public Dictionary<string, string> _statusses;
    public int DanceId;
    public bool IsSpectator;

    public int DiceCounterAmount;
    public int DiceCounter;

    public bool ConstruitEnable = false;
    public bool ConstruitZMode = false;
    public double ConstruitHeigth = 1.0;

    public bool Freeze;
    public int FreezeEndCounter;

    public bool IsTransf;
    public bool TransfBot;
    public string TransfRace;

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

    public Point Coordinate => new(this.X, this.Y);

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

    public bool NeedsAutokick => !this.IsBot && (this.GetClient() == null || this.GetClient().GetUser() == null || (this.GetClient().GetUser().Rank < 2 && this.IdleTime >= 1200));

    public bool IsTrading => !this.IsBot && this.ContainStatus("trd");

    public bool IsBot => this.BotData != null;

    public RolePlayer Roleplayer
    {
        get
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
            {
                return null;
            }

            var RPManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(room.RoomData.OwnerId);
            if (RPManager == null)
            {
                return null;
            }

            var Rp = RPManager.GetPlayer(this.UserId);
            if (Rp == null)
            {
                return null;
            }

            return Rp;
        }
    }

    public Dictionary<string, string> Statusses
    {
        get
        {
            return this._statusses;
        }
    }

    public RoomUser(int userId, int RoomId, int VirtualId, Room room)
    {
        this.Freezed = false;
        this.UserId = userId;
        this.RoomId = RoomId;
        this.VirtualId = VirtualId;
        this.IdleTime = 0;
        this.X = 0;
        this.Y = 0;
        this.Z = 0.0;
        this.RotHead = 0;
        this.RotBody = 0;
        this.UpdateNeeded = true;
        this._statusses = new Dictionary<string, string>();
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

    public bool Equals(RoomUser other)
    {
        if (other == null)
        {
            return false;
        }

        return other.VirtualId == this.VirtualId;
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
        else if (this.GetClient() != null && this.GetClient().GetUser() != null)
        {
            return this.GetClient().GetUser().Username;
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
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
            {
                return false;
            }

            return this.GetUsername() == room.RoomData.OwnerName;
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

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            return;
        }

        room.SendPacket(new SleepComposer(this.VirtualId, false));
    }

    public void Dispose()
    {
        this._statusses.Clear();
        this.IsDispose = true;
        this.Room = null;
        this._client = null;
    }

    public void SendWhisperChat(string message, bool Info = true) => this.GetClient().SendPacket(new WhisperComposer(this.VirtualId, message, Info ? 34 : 0));

    public void OnChatMe(string MessageText, int Color = 0, bool Shout = false)
    {
        if (Shout)
        {
            this.GetClient().SendPacket(new ShoutComposer(this.VirtualId, MessageText, Color));
        }
        else
        {
            this.GetClient().SendPacket(new ChatComposer(this.VirtualId, MessageText, Color));
        }
    }

    public void OnChat(string MessageText, int Color = 0, bool Shout = false)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            return;
        }

        if (Shout)
        {
            room.SendPacketOnChat(new ShoutComposer(this.VirtualId, MessageText, Color), this, true, this.Team == TeamType.NONE && !this.IsBot);
        }
        else
        {
            room.SendPacketOnChat(new ChatComposer(this.VirtualId, MessageText, Color), this, true, this.Team == TeamType.NONE && !this.IsBot);
        }
    }

    public void MoveTo(Point c, bool Override = false) => this.MoveTo(c.X, c.Y, Override);

    public void MoveTo(int pX, int pY, bool pOverride = false)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            return;
        }

        if (!room.GetGameMap().CanWalkState(pX, pY, pOverride) || this.Freeze || !this.AllowMoveTo)
        {
            return;
        }

        this.Unidle();
        if (this.TeleportEnabled)
        {
            room.SendPacket(room.GetRoomItemHandler().TeleportUser(this, new Point(pX, pY), 0, room.GetGameMap().SqAbsoluteHeight(pX, pY)));
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

        this.UpdateNeeded = false;
        this.IsWalking = false;

        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            room.GetGameMap().AddTakingSquare(pX, pY);
        }
    }

    public void SetPos(int pX, int pY, double pZ)
    {
        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            room.GetGameMap().UpdateUserMovement(this.Coordinate, new Point(pX, pY), this);
        }

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

    public void CarryItem(int itemId, bool notTimer = false)
    {
        this.CarryItemID = itemId;
        this.CarryTimer = itemId <= 0 || notTimer ? 0 : 240;

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            return;
        }

        room.SendPacket(new CarryObjectComposer(this.VirtualId, itemId));
    }

    public void SetRot(int Rotation, bool HeadOnly, bool IgnoreWalk = false)
    {
        if (this.ContainStatus("lay") || (this.IsWalking && !IgnoreWalk))
        {
            return;
        }

        var num = this.RotBody - Rotation;
        this.RotHead = this.RotBody;
        if (HeadOnly || this.ContainStatus("sit"))
        {
            if (this.RotBody is 2 or 4)
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
            else if (this.RotBody is 0 or 6)
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
        else if (num is <= (-2) or >= 2)
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

    public bool ContainStatus(string key) => this._statusses.ContainsKey(key);

    public void SetStatus(string Key, string Value)
    {
        if (this._statusses.ContainsKey(Key))
        {
            this._statusses[Key] = Value;
        }
        else
        {
            this._statusses.Add(Key, Value);
        }
    }

    public void RemoveStatus(string Key)
    {
        if (!this._statusses.ContainsKey(Key))
        {
            return;
        }

        this._statusses.Remove(Key);
    }

    public void ApplyEffect(int EffectId, bool DontSave = false)
    {
        if (this.Room == null)
        {
            return;
        }

        if (this.RidingHorse && EffectId != 77 && EffectId != 103)
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

        this.Room.SendPacket(new AvatarEffectComposer(this.VirtualId, EffectId));
    }

    public bool SetPetTransformation(string NamePet, int RaceId)
    {
        var RaceData = TransformUtility.GetRace(NamePet, RaceId);

        if (string.IsNullOrEmpty(RaceData))
        {
            return false;
        }
        else
        {
            this.TransfRace = RaceData;

            return true;
        }
    }

    public GameClient GetClient()
    {
        if (this.IsBot)
        {
            return null;
        }

        if (this._client == null)
        {
            this._client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this.UserId);
        }

        return this._client;
    }
}
