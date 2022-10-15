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
    public GameClient Client { get; private set; }
    public int UserId { get; set; }
    public int VirtualId { get; set; }
    public int RoomId { get; set; }
    public int IdleTime { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public int GoalX { get; set; }
    public int GoalY { get; set; }
    public bool SetStep { get; set; }
    public int SetX { get; set; }
    public int SetY { get; set; }
    public double SetZ { get; set; }
    public int CarryItemID { get; set; }
    public int CarryTimer { get; set; }
    public int RotHead { get; set; }
    public int RotBody { get; set; }
    public bool CanWalk { get; set; }
    public bool AllowOverride { get; set; }
    public bool TeleportEnabled { get; set; }
    public bool AllowMoveToRoller { get; set; }
    public RoomBot BotData { get; set; }
    public Pet PetData { get; set; }
    public BotAI BotAI { get; set; }
    public Room Room { get; set; }

    public ItemEffectType CurrentItemEffect { get; set; }
    public bool Freezed { get; set; }
    public int FreezeCounter { get; set; }
    public TeamType Team { get; set; }
    public FreezePowerUp BanzaiPowerUp { get; set; }
    public int FreezeLives { get; set; }
    public bool ShieldActive { get; set; }
    public int ShieldCounter { get; set; }
    public int CountFreezeBall { get; set; } = 1;
    public bool MoonwalkEnabled { get; set; }
    public bool FacewalkEnabled { get; set; }
    public bool RidingHorse { get; set; }
    public bool IsSit { get; set; }
    public bool IsLay { get; set; }
    public int HorseID { get; set; }
    public bool IsWalking { get; set; }
    public bool UpdateNeeded { get; set; }
    public bool IsAsleep { get; set; }
    public Dictionary<string, string> Statusses { get; set; }
    public int DanceId { get; set; }
    public bool IsSpectator { get; set; }

    public int DiceCounterAmount { get; set; }
    public int DiceCounter { get; set; }

    public bool ConstruitEnable { get; set; }
    public bool ConstruitZMode { get; set; }
    public double ConstruitHeigth { get; set; }

    public bool Freeze { get; set; }
    public int FreezeEndCounter { get; set; }

    public bool IsTransf { get; set; }
    public bool TransfBot { get; set; }
    public string TransfRace { get; set; }

    public bool AllowBall { get; set; }
    public bool MoveWithBall { get; set; }
    public bool SetMoveWithBall { get; set; }
    public bool AllowShoot { get; set; }

    public string ChatTextColor { get; set; }

    public string LastMessage { get; set; }
    public int LastMessageCount { get; set; }

    public int PartyId { get; set; }
    public int TimerResetEffect { get; set; }

    public string LoaderVideoId { get; set; }

    public int WiredPoints { get; set; }
    public bool InGame { get; set; }
    public bool WiredGivelot { get; set; }

    //Walk
    public bool BreakWalkEnable { get; set; }
    public bool StopWalking { get; set; }
    public bool ReverseWalk { get; set; }
    public bool WalkSpeed { get; set; }
    public bool AllowMoveTo { get; set; }

    public int LLPartner { get; set; }

    public int CurrentEffect { get; set; }

    //RP STATS TEMP
    public List<int> AllowBuyItems { get; set; }

    public bool IsDispose { get; set; }
    public int UserTimer { get; set; }

    public List<string> WhiperGroupUsers { get; set; }

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

    public bool NeedsAutokick => !this.IsBot && (this.Client == null || this.Client.User == null || (this.Client.User.Rank < 2 && this.IdleTime >= 1200));

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

            var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(room.RoomData.OwnerId);
            if (rpManager == null)
            {
                return null;
            }

            var rp = rpManager.GetPlayer(this.UserId);
            if (rp == null)
            {
                return null;
            }

            return rp;
        }
    }

    public RoomUser(int userId, int roomId, int virtualId, Room room)
    {
        this.Freezed = false;
        this.UserId = userId;
        this.RoomId = roomId;
        this.VirtualId = virtualId;
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
        this.CurrentItemEffect = ItemEffectType.None;
        this.BreakWalkEnable = false;
        this.AllowShoot = false;
        this.AllowBuyItems = new List<int>();
        this.IsDispose = false;
        this.AllowMoveTo = true;
        this.WhiperGroupUsers = new List<string>();

        if (!this.IsBot)
        {
            this.Client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this.UserId);
        }
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
        else if (this.Client != null && this.Client.User != null)
        {
            return this.Client.User.Username;
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
        this.Statusses.Clear();
        this.IsDispose = true;
        this.Room = null;
        this.Client = null;
    }

    public void SendWhisperChat(string message, bool info = true) => this.Client.SendPacket(new WhisperComposer(this.VirtualId, message, info ? 34 : 0));

    public void OnChatMe(string messageText, int color = 0, bool shout = false)
    {
        if (shout)
        {
            this.Client.SendPacket(new ShoutComposer(this.VirtualId, messageText, color));
        }
        else
        {
            this.Client.SendPacket(new ChatComposer(this.VirtualId, messageText, color));
        }
    }

    public void OnChat(string messageText, int color = 0, bool shout = false)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            return;
        }

        if (shout)
        {
            room.SendPacketOnChat(new ShoutComposer(this.VirtualId, messageText, color), this, true, this.Team == TeamType.None && !this.IsBot);
        }
        else
        {
            room.SendPacketOnChat(new ChatComposer(this.VirtualId, messageText, color), this, true, this.Team == TeamType.None && !this.IsBot);
        }
    }

    public void MoveTo(Point c, bool @override = false) => this.MoveTo(c.X, c.Y, @override);

    public void MoveTo(int pX, int pY, bool pOverride = false)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            return;
        }

        if (!room.GameMap.CanWalkState(pX, pY, pOverride) || this.Freeze || !this.AllowMoveTo)
        {
            return;
        }

        this.Unidle();
        if (this.TeleportEnabled)
        {
            room.SendPacket(RoomItemHandling.TeleportUser(this, new Point(pX, pY), 0, room.GameMap.SqAbsoluteHeight(pX, pY)));
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
            room.GameMap.AddTakingSquare(pX, pY);
        }
    }

    public void SetPos(int pX, int pY, double pZ)
    {
        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.RoomId, out var room))
        {
            room.GameMap.UpdateUserMovement(this.Coordinate, new Point(pX, pY), this);
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

    public void SetRot(int rotation, bool headOnly, bool ignoreWalk = false)
    {
        if (this.ContainStatus("lay") || (this.IsWalking && !ignoreWalk))
        {
            return;
        }

        var num = this.RotBody - rotation;
        this.RotHead = this.RotBody;
        if (headOnly || this.ContainStatus("sit"))
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
            this.RotHead = rotation;
            this.RotBody = rotation;
        }
        else
        {
            this.RotHead = rotation;
        }

        this.UpdateNeeded = true;
    }

    public bool ContainStatus(string key) => this.Statusses.ContainsKey(key);

    public void SetStatus(string key, string value)
    {
        if (this.Statusses.ContainsKey(key))
        {
            this.Statusses[key] = value;
        }
        else
        {
            this.Statusses.Add(key, value);
        }
    }

    public void RemoveStatus(string key)
    {
        if (!this.Statusses.ContainsKey(key))
        {
            return;
        }

        _ = this.Statusses.Remove(key);
    }

    public void ApplyEffect(int effectId, bool dontSave = false)
    {
        if (this.Room == null)
        {
            return;
        }

        if (this.RidingHorse && effectId != 77 && effectId != 103)
        {
            return;
        }

        if (this.CurrentEffect == effectId && !dontSave)
        {
            return;
        }

        if (!dontSave)
        {
            this.CurrentEffect = effectId;
        }

        this.Room.SendPacket(new AvatarEffectComposer(this.VirtualId, effectId));
    }

    public bool SetPetTransformation(string namePet, int raceId)
    {
        var raceData = TransformUtility.GetRace(namePet, raceId);

        if (string.IsNullOrEmpty(raceData))
        {
            return false;
        }
        else
        {
            this.TransfRace = raceData;

            return true;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is not RoomUser)
        {
            return false;
        }

        return ((RoomUser)obj).VirtualId == this.VirtualId;
    }

    public override int GetHashCode() => this.VirtualId;
    public bool Equals(RoomUser other) => other.VirtualId == this.VirtualId;
}
