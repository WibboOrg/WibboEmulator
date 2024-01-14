namespace WibboEmulator.Games.Rooms;

using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.Groups;

public class RoomData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string OwnerName { get; set; }
    public int OwnerId { get; set; }
    public Language Langue { get; set; }
    public string Password { get; set; }
    public RoomAccess Access { get; set; }
    public int Category { get; set; }
    public int UsersNow { get; set; }
    public int UsersMax { get; set; }
    public string ModelName { get; set; }
    public int Score { get; set; }
    public List<string> Tags { get; set; }
    public bool AllowPets { get; set; }
    public bool AllowPetsEating { get; set; }
    public bool AllowWalkthrough { get; set; }
    public bool AllowRightsOverride { get; set; }
    public bool HideWall { get; set; }
    public string Wallpaper { get; set; }
    public string Floor { get; set; }
    public string Landscape { get; set; }
    public RoomModel Model { get; set; }
    public int WallThickness { get; set; }
    public int FloorThickness { get; set; }
    public int MuteFuse { get; set; }
    public int BanFuse { get; set; }
    public int WhoCanKick { get; set; }
    public int ChatType { get; set; }
    public int ChatBalloon { get; set; }
    public int ChatSpeed { get; set; }
    public int ChatMaxDistance { get; set; }
    public int ChatFloodProtection { get; set; }
    public int GroupId { get; set; }
    public int TrocStatus { get; set; }
    public Group Group { get; set; }
    public bool HideWireds { get; set; }
    public int SellPrice { get; set; }
    public bool WiredSecurity { get; set; }
    public int TagCount => this.Tags.Count;

    public RoomData()
    {
    }

    public void FillNull(int id)
    {
        this.Id = id;
        this.Name = "Unknown Room";
        this.Description = "-";
        this.OwnerName = "-";
        this.Category = 0;
        this.UsersNow = 0;
        this.UsersMax = 0;
        this.ModelName = "model_a";
        this.Score = 0;
        this.Tags = new List<string>();
        this.AllowPets = true;
        this.AllowPetsEating = false;
        this.AllowWalkthrough = true;
        this.HideWall = false;
        this.Password = "";
        this.Wallpaper = "0.0";
        this.Floor = "0.0";
        this.Landscape = "0.0";
        this.WallThickness = 0;
        this.FloorThickness = 0;
        this.MuteFuse = 0;
        this.WhoCanKick = 0;
        this.BanFuse = 0;
        this.ChatType = 0;
        this.ChatBalloon = 0;
        this.ChatSpeed = 0;
        this.ChatMaxDistance = 0;
        this.ChatFloodProtection = 0;
        this.GroupId = 0;
        this.TrocStatus = 2;
        this.Group = null;
        this.AllowRightsOverride = false;
        this.Model = WibboEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, id);
        this.HideWireds = false;
        this.SellPrice = 0;
    }

    public void Fill(RoomEntity room)
    {
        this.Id = room.Id;
        this.Name = room.Caption;
        this.Description = room.Description;
        this.OwnerName = room.Owner;
        this.OwnerId = room.OwnerId;
        this.Langue = LanguageManager.ParseLanguage(room.Langue);

        this.Access = room.State switch
        {
            RoomState.Open => RoomAccess.Open,
            RoomState.Password => RoomAccess.Password,
            RoomState.Hide => RoomAccess.Invisible,
            RoomState.Locked => RoomAccess.Doorbell,
            _ => RoomAccess.Open
        };
        this.Category = room.Category;
        this.UsersNow = room.UsersNow;
        this.UsersMax = room.UsersMax;
        this.ModelName = room.ModelName;
        this.Score = room.Score;
        this.Tags = new List<string>();
        this.AllowPets = room.AllowPets;
        this.AllowPetsEating = room.AllowPetsEat;
        this.AllowWalkthrough = room.AllowWalkthrough;
        this.AllowRightsOverride = room.AllowRightsOverride;
        this.HideWall = room.AllowHideWall;
        this.Password = room.Password;
        this.Wallpaper = room.Wallpaper;
        this.Floor = room.Floor;
        this.Landscape = room.Landscape;
        this.FloorThickness = room.FloorThick;
        this.WallThickness = room.WallThick;

        this.ChatType = room.ChatType;
        this.ChatBalloon = room.ChatBalloon;
        this.ChatSpeed = room.ChatSpeed;
        this.ChatMaxDistance = room.ChatMaxDistance;
        this.ChatFloodProtection = room.ChatFloodProtection;

        this.MuteFuse = room.ModerationMuteFuse;
        this.WhoCanKick = room.ModerationKickFuse;
        this.BanFuse = room.ModerationBanFuse;
        this.GroupId = room.GroupId;
        if (WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(this.GroupId, out var group))
        {
            this.Group = group;
        }
        this.HideWireds = room.AllowHideWireds;
        this.TrocStatus = room.TrocStatus;

        var tags = room.Tags;

        if (tags != null)
        {
            foreach (var tag in tags.Split(','))
            {
                this.Tags.Add(tag);
            }
        }

        this.SellPrice = room.Price;
        this.WiredSecurity = room.WiredSecurity;
        this.Model = WibboEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, this.Id);
    }
}
