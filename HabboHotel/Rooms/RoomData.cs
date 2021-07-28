using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Core;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id;
        public string Name;
        public string Description;
        public string OwnerName;
        public int OwnerId;
        public Language Langue;
        public string Password;
        public int State;
        public int Category;
        public int UsersNow;
        public int UsersMax;
        public string ModelName;
        public int Score;
        public List<string> Tags;
        public bool AllowPets;
        public bool AllowPetsEating;
        public bool AllowWalkthrough;
        public bool AllowRightsOverride;
        public bool Hidewall;
        public string Wallpaper;
        public string Floor;
        public string Landscape;
        private RoomModel mModel;
        public int WallThickness;
        public int FloorThickness;
        public int MuteFuse;
        public int BanFuse;
        public int WhoCanKick;
        public int ChatType;
        public int ChatBalloon;
        public int ChatSpeed;
        public int ChatMaxDistance;
        public int ChatFloodProtection;
        public int GroupId;
        public int TrocStatus;
        public Group Group;
        public bool HideWireds;
        public int SellPrice;
        public int TagCount => this.Tags.Count;

        public RoomModel Model
        {
            get
            {
                if (this.mModel == null)
                {
                    this.mModel = ButterflyEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, this.Id);
                }

                return this.mModel;
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
            this.mModel = ButterflyEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, pId);
            this.HideWireds = false;
            this.SellPrice = 0;
        }

        public void Fill(DataRow Row)
        {
            this.Id = Convert.ToInt32(Row["id"]);
            this.Name = (string)Row["caption"];
            this.Description = (string)Row["description"];
            this.OwnerName = (string)Row["owner"];
            this.OwnerId = 0;
            this.Langue = Language.FRANCAIS;

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id, langue FROM users WHERE username = @owner");
                queryreactor.AddParameter("owner", this.OwnerName);
                DataRow UserRow = queryreactor.GetRow();
                if (UserRow != null)
                {
                    this.OwnerId = Convert.ToInt32(UserRow["id"]);
                    this.Langue = LanguageManager.ParseLanguage(UserRow["langue"].ToString());
                }
            }

            switch (Row["state"].ToString().ToLower())
            {
                case "open":
                    this.State = 0;
                    break;
                case "password":
                    this.State = 2;
                    break;
                case "hide":
                    this.State = 3;
                    break;
                default:
                    this.State = 1;
                    break;
            }

            this.Category = Convert.ToInt32(Row["category"]);
            this.UsersNow = Convert.ToInt32(Row["users_now"]);
            this.UsersMax = Convert.ToInt32(Row["users_max"]);
            this.ModelName = (string)Row["model_name"];
            this.Score = Convert.ToInt32(Row["score"]);
            this.Tags = new List<string>();
            this.AllowPets = ButterflyEnvironment.EnumToBool(Row["allow_pets"].ToString());
            this.AllowPetsEating = ButterflyEnvironment.EnumToBool(Row["allow_pets_eat"].ToString());
            this.AllowWalkthrough = ButterflyEnvironment.EnumToBool(Row["allow_walkthrough"].ToString());
            this.AllowRightsOverride = ButterflyEnvironment.EnumToBool(Row["allow_rightsoverride"].ToString());
            this.Hidewall = ButterflyEnvironment.EnumToBool(Row["allow_hidewall"].ToString());
            this.Password = (string)Row["password"];
            this.Wallpaper = (string)Row["wallpaper"];
            this.Floor = (string)Row["floor"];
            this.Landscape = (string)Row["landscape"];
            this.FloorThickness = Convert.ToInt32(Row["floorthick"]);
            this.WallThickness = Convert.ToInt32(Row["wallthick"]);

            this.ChatType = Convert.ToInt32(Row["chat_type"]);
            this.ChatBalloon = Convert.ToInt32(Row["chat_balloon"]);
            this.ChatSpeed = Convert.ToInt32(Row["chat_speed"]);
            this.ChatMaxDistance = Convert.ToInt32(Row["chat_max_distance"]);
            this.ChatFloodProtection = Convert.ToInt32(Row["chat_flood_protection"]);

            this.MuteFuse = Convert.ToInt32((string)Row["moderation_mute_fuse"]);
            this.WhoCanKick = Convert.ToInt32((string)Row["moderation_kick_fuse"]);
            this.BanFuse = Convert.ToInt32((string)Row["moderation_ban_fuse"]);
            this.GroupId = Convert.ToInt32(Row["group_id"]);
            ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(this.GroupId, out Group Group);
            this.Group = Group;
            this.HideWireds = ButterflyEnvironment.EnumToBool(Row["allow_hidewireds"].ToString());

            this.TrocStatus = Convert.ToInt32((string)Row["troc_status"]);

            string str3 = Row["tags"].ToString();
            char[] chArray1 = new char[1] { ',' };
            foreach (string str1 in str3.Split(chArray1))
            {
                this.Tags.Add(str1);
            }

            this.SellPrice = Convert.ToInt32(Row["price"]);

            this.mModel = ButterflyEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, this.Id);
        }
    }
}
