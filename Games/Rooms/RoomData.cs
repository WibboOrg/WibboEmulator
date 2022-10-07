namespace WibboEmulator.Games.Rooms;
using System.Data;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.User;
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
    public int State { get; set; }
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
    public bool Hidewall { get; set; }
    public string Wallpaper { get; set; }
    public string Floor { get; set; }
    public string Landscape { get; set; }
    private RoomModel _roomModel;
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
    public int TagCount => this.Tags.Count;

    public RoomAccess Access { get; set; }

    public RoomModel Model
    {
        get
        {
            this._roomModel ??= WibboEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, this.Id);

            return this._roomModel;
        }
    }

    public RoomData()
    {
    }

    public void FillNull(int pId)
    {
        this.Id = pId;
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
        this.Hidewall = false;
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
        this._roomModel = WibboEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, pId);
        this.HideWireds = false;
        this.SellPrice = 0;
    }

    public void Fill(DataRow row)
    {
        this.Id = Convert.ToInt32(row["id"]);
        this.Name = (string)row["caption"];
        this.Description = (string)row["description"];
        this.OwnerName = (string)row["owner"];
        this.OwnerId = 0;
        this.Langue = Language.FRANCAIS;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            var userRow = UserDao.GetIdAndLangue(dbClient, this.OwnerName);
            if (userRow != null)
            {
                this.OwnerId = Convert.ToInt32(userRow["id"]);
                this.Langue = LanguageManager.ParseLanguage(userRow["langue"].ToString());
            }
        }

        var state = row["state"].ToString();

        this.State = (state?.ToLower()) switch
        {
            "open" => 0,
            "password" => 2,
            "hide" => 3,
            _ => 1,
        };
        this.Category = Convert.ToInt32(row["category"]);
        this.UsersNow = Convert.ToInt32(row["users_now"]);
        this.UsersMax = Convert.ToInt32(row["users_max"]);
        this.ModelName = (string)row["model_name"];
        this.Score = Convert.ToInt32(row["score"]);
        this.Tags = new List<string>();
        this.AllowPets = WibboEnvironment.EnumToBool(row["allow_pets"].ToString());
        this.AllowPetsEating = WibboEnvironment.EnumToBool(row["allow_pets_eat"].ToString());
        this.AllowWalkthrough = WibboEnvironment.EnumToBool(row["allow_walkthrough"].ToString());
        this.AllowRightsOverride = WibboEnvironment.EnumToBool(row["allow_rightsoverride"].ToString());
        this.Hidewall = WibboEnvironment.EnumToBool(row["allow_hidewall"].ToString());
        this.Password = (string)row["password"];
        this.Wallpaper = (string)row["wallpaper"];
        this.Floor = (string)row["floor"];
        this.Landscape = (string)row["landscape"];
        this.FloorThickness = Convert.ToInt32(row["floorthick"]);
        this.WallThickness = Convert.ToInt32(row["wallthick"]);

        this.ChatType = Convert.ToInt32(row["chat_type"]);
        this.ChatBalloon = Convert.ToInt32(row["chat_balloon"]);
        this.ChatSpeed = Convert.ToInt32(row["chat_speed"]);
        this.ChatMaxDistance = Convert.ToInt32(row["chat_max_distance"]);
        this.ChatFloodProtection = Convert.ToInt32(row["chat_flood_protection"]);

        this.MuteFuse = Convert.ToInt32((string)row["moderation_mute_fuse"]);
        this.WhoCanKick = Convert.ToInt32((string)row["moderation_kick_fuse"]);
        this.BanFuse = Convert.ToInt32((string)row["moderation_ban_fuse"]);
        this.GroupId = Convert.ToInt32(row["group_id"]);
        _ = WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(this.GroupId, out var group);
        this.Group = group;
        this.HideWireds = WibboEnvironment.EnumToBool(row["allow_hidewireds"].ToString());
        this.TrocStatus = Convert.ToInt32((string)row["troc_status"]);

        var tags = row["tags"].ToString();

        if (tags != null)
        {
            foreach (var tag in tags.Split(','))
            {
                this.Tags.Add(tag);
            }
        }

        this.SellPrice = Convert.ToInt32(row["price"]);
        this._roomModel = WibboEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, this.Id);
    }
}
