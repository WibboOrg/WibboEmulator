using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Roleplay.Player;
using WibboEmulator.Game.Rooms.Games;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Conditions
{
    public class SuperWiredCondition : WiredConditionBase, IWiredCondition, IWired
    {
        public SuperWiredCondition(Item item, Room room) : base(item, room, (int)WiredConditionType.ACTOR_IS_WEARING_BADGE)
        {
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems(inDatabase);

            if(inDatabase) return;

            this.CheckPermission();
        }

        private void CheckPermission()
        {
            string effet;
            if (this.StringParam.Contains(':'))
            {
                effet = this.StringParam.Split(':')[0].ToLower();
            }
            else
            {
                effet = this.StringParam.ToLower();
            }

            switch (effet)
            {
                case "enemy":
                case "work":
                case "notwork":
                case "moneyplus":
                case "moneymoins":
                case "money1plus":
                case "money1moins":
                case "money2plus":
                case "money2moins":
                case "money3plus":
                case "money3moins":
                case "money4plus":
                case "money4moins":
                case "levelplus":
                case "levelmoins":
                case "healthplus":
                case "healthmoins":
                case "health":
                case "dead":
                case "notdead":
                case "munition":
                case "munitionplus":
                case "munitionmoins":
                case "weaponfarid":
                case "notweaponfarid":
                case "weaponcacid":
                case "notweaponcacid":
                case "energyplus":
                case "energymoins":
                case "hygieneplus":
                case "hygienemoins":
                case "inventoryitem":
                case "inventorynotitem":
                case "rphourplus":
                case "rphourmoins":
                case "rphour":
                case "rpminuteplus":
                case "rpminutemoins":
                case "rpminute":
                case "winusermoney":
                case "notwinusermoney":
                    if (this.RoomInstance.IsRoleplay)
                    {
                        return;
                    }
                    break;

                case "rankplus":
                case "rankmoin":
                case "rank":
                    if (this.IsStaff)
                    {
                        return;
                    }
                    break;
                case "favogroupid":
                case "notfavogroupid":
                case "mission":
                case "notmission":
                case "missioncontais":
                case "notmissioncontais":
                case "usergirl":
                case "notusergirl":
                case "userboy":
                case "notuserboy":
                case "namebot":
                case "notnamebot":
                case "badge":
                case "notbadge":
                case "handitem":
                case "nothanditem":
                case "enable":
                case "notenable":
                case "username":
                case "notusername":
                case "transf":
                case "nottransf":
                case "userteam":
                case "usernotteam":
                case "ingroup":
                case "innotgroup":
                case "rot":
                case "notrot":
                case "lay":
                case "notlay":
                case "sit":
                case "notsit":
                case "usertimer":
                case "usertimerplus":
                case "usertimermoins":
                case "point":
                case "pointplus":
                case "pointmoins":
                case "ingame":
                case "notingame":
                case "freeze":
                case "notfreeze":
                case "winteam":
                case "notwinteam":
                case "allowshoot":
                case "notallowshoot":
                case "isbot":
                case "notisbot":

                case "roomopen":
                case "roomnotopen":
                case "roomclose":
                case "roomnotclose":
                case "teamredcount":
                case "teamrednotcount":
                case "teamyellowcount":
                case "teamyellownotcount":
                case "teambluecount":
                case "teambluenotcount":
                case "teamgreencount":
                case "teamgreennotcount":
                case "teamallcount":
                case "teamallnotcount":

                case "itemmode":
                case "itemnotmode":
                case "itemrot":
                case "itemnotrot":
                case "itemdistanceplus":
                case "itemdistancemoins":
                case "winuserpoint":
                case "notwinuserpoint":
                    return;
            }

            this.StringParam = "";
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (this.StringParam == "")
            {
                return false;
            }

            string Value = "";


            string Effect;
            if (this.StringParam.Contains(':'))
            {
                Effect = this.StringParam.Split(':')[0].ToLower();
                Value = this.StringParam.Split(':')[1];
            }
            else
            {
                Effect = this.StringParam;
            }

            bool Bool = false;
            if (user != null)
            {
                Bool = this.UserCommand(user, this.RoomInstance, Effect, Value);
            }

            if (Bool == false)
            {
                Bool = this.RoomCommand(this.RoomInstance, Effect, Value);
            }

            if (Bool == false)
            {
                Bool = this.RpUserCommand(user, Effect, Value);
            }

            if (Bool == false)
            {
                Bool = this.RpGlobalCommand(this.RoomInstance, Effect, Value);
            }

            if (Bool == false && TriggerItem != null)
            {
                Bool = this.ItemCommand(TriggerItem, user, Effect, Value);
            }

            if (Effect.Contains("not"))
            {
                Bool = !Bool;
            }

            return Bool;
        }

        private bool RpGlobalCommand(Room Room, string Effect, string Value)
        {
            if (Room == null || !Room.IsRoleplay)
            {
                return false;
            }

            bool Result = false;
            switch (Effect)
            {
                case "rpminuteplus":
                    {
                        if (!int.TryParse(Value, out int ValueInt))
                        {
                            break;
                        }

                        if (Room.Roleplay.Minute >= ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rpminutemoins":
                    {
                        if (!int.TryParse(Value, out int ValueInt))
                        {
                            break;
                        }

                        if (Room.Roleplay.Minute < ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rpminute":
                    {
                        if (!int.TryParse(Value, out int ValueInt))
                        {
                            break;
                        }

                        if (Room.Roleplay.Minute == ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rphourplus":
                    {
                        if (!int.TryParse(Value, out int ValueInt))
                        {
                            break;
                        }

                        if (Room.Roleplay.Hour >= ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rphourmoins":
                    {
                        if (!int.TryParse(Value, out int ValueInt))
                        {
                            break;
                        }

                        if (Room.Roleplay.Hour < ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rphour":
                    {
                        if (!int.TryParse(Value, out int ValueInt))
                        {
                            break;
                        }

                        if (Room.Roleplay.Hour == ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "enemy":
                    {
                        string[] Params = Value.Split(';');
                        if (Params.Length != 3)
                        {
                            break;
                        }

                        RoomUser BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(Params[0]);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                        {
                            break;
                        }

                        switch (Params[1])
                        {
                            case "dead":
                                {
                                    if (BotOrPet.BotData.RoleBot.Dead && Params[2] == "true")
                                    {
                                        Result = true;
                                    }

                                    if (!BotOrPet.BotData.RoleBot.Dead && Params[2] == "false")
                                    {
                                        Result = true;
                                    }

                                    break;
                                }
                            case "aggro":
                                {
                                    if (BotOrPet.BotData.RoleBot.AggroVirtuelId > 0 && Params[2] == "true")
                                    {
                                        Result = true;
                                    }

                                    if (BotOrPet.BotData.RoleBot.AggroVirtuelId == 0 && Params[2] == "false")
                                    {
                                        Result = true;
                                    }

                                    break;
                                }
                        }
                        break;
                    }
            }

            return Result;
        }

        private bool RpUserCommand(RoomUser User, string Effect, string Value)
        {
            Room Room = this.RoomInstance;
            if (Room == null || !Room.IsRoleplay)
            {
                return false;
            }

            if (User == null || User.GetClient() == null || User.GetClient().GetUser() == null)
            {
                return false;
            }

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null)
            {
                return false;
            }

            bool Result = false;
            switch (Effect)
            {
                case "inventoryitem":
                case "inventorynotitem":
                    {
                        if (!int.TryParse(Value, out int ValueInt))
                        {
                            break;
                        }

                        if (Rp.GetInventoryItem(ValueInt) != null)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "energyplus":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (Rp.Energy >= ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "energymoins":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (Rp.Energy < ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "munition":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (Rp.Munition == ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "munitionplus":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (Rp.Munition >= ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "munitionmoins":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (Rp.Munition < ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "moneyplus":
                    {
                        int.TryParse(Value, out int ValueInt);
                        if (Rp.Money >= ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "moneymoins":
                    {
                        int.TryParse(Value, out int ValueInt);
                        if (Rp.Money < ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "levelplus":
                    {
                        int.TryParse(Value, out int ValueInt);
                        if (Rp.Level >= ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "levelmoins":
                    {
                        int.TryParse(Value, out int ValueInt);
                        if (Rp.Level < ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "healthplus":
                    {
                        int.TryParse(Value, out int ValueInt);
                        if (Rp.Health >= ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "healthmoins":
                    {
                        int.TryParse(Value, out int ValueInt);
                        if (Rp.Health < ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "health":
                    {
                        int.TryParse(Value, out int ValueInt);
                        if (Rp.Health == ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "dead":
                case "notdead":
                    {
                        if (Rp.Dead == true)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "weaponfarid":
                case "notweaponfarid":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (Rp.WeaponGun.Id == ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "weaponcacid":
                case "notweaponcacid":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (Rp.WeaponCac.Id == ValueInt)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "winusermoney":
                case "notwinusermoney":
                    {
                        Result = true;

                        foreach (RoomUser UserSearch in Room.GetRoomUserManager().GetRoomUsers())
                        {
                            if (UserSearch == null)
                            {
                                continue;
                            }

                            if (UserSearch == User)
                            {
                                continue;
                            }

                            if (UserSearch.Roleplayer == null)
                            {
                                continue;
                            }

                            if (UserSearch.Roleplayer.Money < Rp.Money)
                            {
                                continue;
                            }

                            Result = false;
                            break;
                        }

                        break;
                    }
            }
            return Result;
        }

        private bool ItemCommand(Item item, RoomUser User, string Effect, string Value)
        {
            bool Bool = false;
            switch (Effect)
            {
                case "itemmode":
                case "itemnotmode":
                    {
                        if (int.TryParse(item.ExtraData, out int Num))
                        {
                            if (item.ExtraData == Value)
                            {
                                Bool = true;
                            }
                        }
                        break;
                    }
                case "itemrot":
                case "itemnotrot":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (item.Rotation == ValueInt)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "itemdistanceplus":
                    {
                        if (User == null)
                        {
                            break;
                        }

                        int.TryParse(Value, out int ValueInt);

                        if (Math.Abs(User.X - item.X) >= ValueInt && Math.Abs(User.Y - item.Y) >= ValueInt)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "itemdistancemoins":
                    {
                        if (User == null)
                        {
                            break;
                        }

                        int.TryParse(Value, out int ValueInt);

                        if (Math.Abs(User.X - item.X) <= ValueInt && Math.Abs(User.Y - item.Y) <= ValueInt)
                        {
                            Bool = true;
                        }

                        break;
                    }
            }

            return Bool;
        }

        private bool RoomCommand(Room room, string Effect, string Value)
        {
            if (room == null)
            {
                return false;
            }

            bool Bool = false;
            switch (Effect)
            {
                case "roomopen":
                case "roomnotopen":
                    {
                        if (room.RoomData.State == 0)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "roomclose":
                case "roomnotclose":
                    {
                        if (room.RoomData.State == 1)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "teamallcount":
                case "teamallnotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int.TryParse(Value, out int Count);
                        if (TeamManager.GetAllPlayer().Count == Count)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "teamredcount":
                case "teamrednotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int.TryParse(Value, out int Count);
                        if (TeamManager.RedTeam.Count == Count)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "teamyellowcount":
                case "teamyellownotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int.TryParse(Value, out int Count);
                        if (TeamManager.YellowTeam.Count == Count)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "teambluecount":
                case "teambluenotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int.TryParse(Value, out int Count);
                        if (TeamManager.BlueTeam.Count == Count)
                        {
                            Bool = true;
                        }

                        break;
                    }
                case "teamgreencount":
                case "teamgreennotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int.TryParse(Value, out int Count);
                        if (TeamManager.GreenTeam.Count == Count)
                        {
                            Bool = true;
                        }

                        break;
                    }
            }
            return Bool;
        }

        private bool UserCommand(RoomUser user, Room Room, string Effect, string Value)
        {
            bool Result = false;
            switch (Effect)
            {
                case "winuserpoint":
                case "notwinuserpoint":
                    {
                        Result = true;

                        foreach (RoomUser UserSearch in Room.GetRoomUserManager().GetRoomUsers())
                        {
                            if (UserSearch == null)
                            {
                                continue;
                            }

                            if (UserSearch == user)
                            {
                                continue;
                            }

                            if (UserSearch.WiredPoints < user.WiredPoints)
                            {
                                continue;
                            }

                            Result = false;
                            break;
                        }

                        break;
                    }
                case "missioncontais":
                case "notmissioncontais":
                    {
                        if (!user.IsBot && user.GetClient().GetUser().Motto.Contains(Value))
                        {
                            Result = true;
                        }

                        break;
                    }
                case "mission":
                case "notmission":
                    {
                        if (!user.IsBot && user.GetClient().GetUser().Motto == Value)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "favogroupid":
                case "notfavogroupid":
                    {
                        int.TryParse(Value, out int GroupId);

                        if (!user.IsBot && user.GetClient().GetUser().FavouriteGroupId == GroupId)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "usergirl":
                case "notusergirl":
                    {
                        if (!user.IsBot && user.GetClient().GetUser().Gender.ToUpper() == "F")
                        {
                            Result = true;
                        }

                        break;
                    }
                case "userboy":
                case "notuserboy":
                    {
                        if (!user.IsBot && user.GetClient().GetUser().Gender.ToUpper() == "M")
                        {
                            Result = true;
                        }

                        break;
                    }
                case "namebot":
                case "notnamebot":
                    {
                        if (user.IsBot && user.BotData.Name == Value)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "isbot":
                case "notisbot":
                    {
                        if (user.IsBot)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "allowshoot":
                case "notallowshoot":
                    {
                        if (user.AllowShoot)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "winteam":
                case "notwinteam":
                    {
                        if (user.Team == TeamType.NONE)
                        {
                            break;
                        }

                        Room room = this.RoomInstance;
                        if (room == null)
                        {
                            break;
                        }

                        TeamType winningTeam = room.GetGameManager().GetWinningTeam();
                        if (user.Team == winningTeam)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "freeze":
                case "notfreeze":
                    {
                        if (user.Freeze)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "ingame":
                case "notingame":
                    {
                        if (user.InGame)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "usertimer":
                    {
                        int.TryParse(Value, out int Points);

                        if (user.UserTimer == Points)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "usertimerplus":
                    {
                        int.TryParse(Value, out int Points);

                        if (user.UserTimer > Points)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "usertimermoins":
                    {
                        int.TryParse(Value, out int point);

                        if (user.UserTimer < point)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "point":
                    {
                        int.TryParse(Value, out int point);

                        if (user.WiredPoints == point)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "pointplus":
                    {
                        int.TryParse(Value, out int Points);

                        if (user.WiredPoints > Points)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "pointmoins":
                    {
                        int.TryParse(Value, out int Points);

                        if (user.WiredPoints < Points)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "ingroup":
                case "innotgroup":
                    {
                        int.TryParse(Value, out int GroupId);

                        if (GroupId == 0)
                        {
                            break;
                        }

                        if (user.IsBot || user.GetClient() != null && user.GetClient().GetUser() != null && user.GetClient().GetUser().MyGroups.Contains(GroupId))
                        {
                            Result = true;
                        }

                        break;
                    }
                case "userteam":
                case "usernotteam":
                    {
                        if (user.Team != TeamType.NONE)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "sit":
                case "notsit":
                    {
                        if (user.ContainStatus("sit"))
                        {
                            Result = true;
                        }

                        break;
                    }
                case "lay":
                case "notlay":
                    {
                        if (user.ContainStatus("lay"))
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rot":
                case "notrot":
                    {
                        if (user.RotBody.ToString() == Value)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "transf":
                case "nottransf":
                    {
                        if (user.IsTransf)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "username":
                case "notusername":
                    {
                        if (user.GetUsername().ToLower() == Value.ToLower())
                        {
                            Result = true;
                        }

                        break;
                    }
                case "handitem":
                case "nothanditem":
                    {
                        if (user.CarryItemID.ToString() == Value)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "badge":
                case "notbadge":
                    {
                        if (user.IsBot || user.GetClient() == null || user.GetClient().GetUser() == null || user.GetClient().GetUser().GetBadgeComponent() == null)
                        {
                            break;
                        }

                        if (user.GetClient().GetUser().GetBadgeComponent().HasBadge(Value))
                        {
                            Result = true;
                        }

                        break;
                    }
                case "enable":
                case "notenable":
                    {
                        if (user.CurrentEffect.ToString() == Value)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rank":
                    {
                        if (user.IsBot)
                        {
                            break;
                        }

                        if (user.GetClient().GetUser().Rank.ToString() == Value)
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rankplus":
                    {
                        if (user.IsBot)
                        {
                            break;
                        }

                        if (user.GetClient().GetUser().Rank > Convert.ToInt32(Value))
                        {
                            Result = true;
                        }

                        break;
                    }
                case "rankmoin":
                    {
                        if (user.IsBot)
                        {
                            break;
                        }

                        if (user.GetClient().GetUser().Rank < Convert.ToInt32(Value))
                        {
                            Result = true;
                        }

                        break;
                    }
            }
            return Result;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.StringParam = row["trigger_data"].ToString();
        }
    }
}
