namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Drawing;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class SuperWiredCondition : WiredConditionBase, IWiredCondition, IWired
{
    public SuperWiredCondition(Item item, Room room) : base(item, room, (int)WiredConditionType.ACTOR_IS_WEARING_BADGE)
    {
    }

    public override void LoadItems(bool inDatabase = false)
    {
        base.LoadItems(inDatabase);

        if (inDatabase)
        {
            return;
        }

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
            case "slotwinner":
            case "notslotwinner":
            case "slotspin":
            case "notslotspin":
            case "slot":
            case "notslot":
                if (this.IsStaff)
                {
                    return;
                }
                break;
            case "favogroupid":
            case "notfavogroupid":
            case "mission":
            case "notmission":
            case "missioncontains":
            case "notmissioncontains":
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
            case "notpoint":
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
            case "classement":
            case "notclassement":
            case "ptsclassementplus":
            case "ptsclassementmoins":
            case "ptsclassement":
            case "notptsclassement":
            case "itemissurvive":
            case "itemisnotsurvive":
                return;
        }

        this.StringParam = "";
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (this.StringParam == "")
        {
            return false;
        }

        string effect;
        string value;
        if (this.StringParam.Contains(':'))
        {
            effect = this.StringParam.Split(':')[0].ToLower();
            value = this.StringParam.Split(':')[1];
        }
        else
        {
            effect = this.StringParam;
            value = string.Empty;
        }

        var result = false;
        if (user != null)
        {
            result = UserCommand(user, this.RoomInstance, effect, value);
        }

        if (result == false)
        {
            result = RoomCommand(this.RoomInstance, effect, value);
        }

        if (result == false)
        {
            result = RpUserCommand(this.RoomInstance, user, effect, value);
        }

        if (result == false)
        {
            result = RpGlobalCommand(this.RoomInstance, effect, value);
        }

        if (result == false && item != null)
        {
            result = ItemCommand(this.RoomInstance, item, user, effect, value);
        }

        if (effect.Contains("not"))
        {
            result = !result;
        }

        return result;
    }

    private static bool RpGlobalCommand(Room room, string effect, string value)
    {
        if (!room.IsRoleplay)
        {
            return false;
        }

        var result = false;
        switch (effect)
        {
            case "rpminuteplus":
            {
                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                if (room.RoomRoleplay.Minute >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "rpminutemoins":
            {
                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                if (room.RoomRoleplay.Minute < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "rpminute":
            {
                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                if (room.RoomRoleplay.Minute == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "rphourplus":
            {
                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                if (room.RoomRoleplay.Hour >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "rphourmoins":
            {
                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                if (room.RoomRoleplay.Hour < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "rphour":
            {
                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                if (room.RoomRoleplay.Hour == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "enemy":
            {
                var parameters = value.Split(';');
                if (parameters.Length != 3)
                {
                    break;
                }

                var botOrPet = room.RoomUserManager.GetBotOrPetByName(parameters[0]);
                if (botOrPet == null || botOrPet.BotData == null || botOrPet.BotData.RoleBot == null)
                {
                    break;
                }

                switch (parameters[1])
                {
                    case "dead":
                    {
                        if (botOrPet.BotData.RoleBot.Dead && parameters[2] == "true")
                        {
                            result = true;
                        }

                        if (!botOrPet.BotData.RoleBot.Dead && parameters[2] == "false")
                        {
                            result = true;
                        }

                        break;
                    }
                    case "aggro":
                    {
                        if (botOrPet.BotData.RoleBot.AggroVirtuelId > 0 && parameters[2] == "true")
                        {
                            result = true;
                        }

                        if (botOrPet.BotData.RoleBot.AggroVirtuelId == 0 && parameters[2] == "false")
                        {
                            result = true;
                        }

                        break;
                    }
                }
                break;
            }
        }

        return result;
    }

    private static bool RpUserCommand(Room room, RoomUser user, string effect, string value)
    {
        if (!room.IsRoleplay)
        {
            return false;
        }

        if (user == null || user.Client == null || user.Client.User == null)
        {
            return false;
        }

        var rp = user.Roleplayer;
        if (rp == null)
        {
            return false;
        }

        var result = false;
        switch (effect)
        {
            case "inventoryitem":
            case "inventorynotitem":
            {
                var itemId = 0;
                var quantity = 1;

                if (value.Contains(';'))
                {
                    if (!int.TryParse(value.Split(';')[0], out itemId) || !int.TryParse(value.Split(';')[1], out quantity))
                    {
                        break;
                    }
                }
                else if (!int.TryParse(value, out itemId))
                {
                    break;
                }

                var itemRp = rp.GetInventoryItem(itemId);
                if (itemRp != null && itemRp.Count >= quantity)
                {
                    result = true;
                }

                break;
            }
            case "energyplus":
            {
                _ = int.TryParse(value, out var valueInt);

                if (rp.Energy >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "energymoins":
            {
                _ = int.TryParse(value, out var valueInt);

                if (rp.Energy < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "munition":
            {
                _ = int.TryParse(value, out var valueInt);

                if (rp.Munition == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "munitionplus":
            {
                _ = int.TryParse(value, out var valueInt);

                if (rp.Munition >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "munitionmoins":
            {
                _ = int.TryParse(value, out var valueInt);

                if (rp.Munition < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "moneyplus":
            {
                _ = int.TryParse(value, out var valueInt);
                if (rp.Money >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "moneymoins":
            {
                _ = int.TryParse(value, out var valueInt);
                if (rp.Money < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "levelplus":
            {
                _ = int.TryParse(value, out var valueInt);
                if (rp.Level >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "levelmoins":
            {
                _ = int.TryParse(value, out var valueInt);
                if (rp.Level < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "healthplus":
            {
                _ = int.TryParse(value, out var valueInt);
                if (rp.Health >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "healthmoins":
            {
                _ = int.TryParse(value, out var valueInt);
                if (rp.Health < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "health":
            {
                _ = int.TryParse(value, out var valueInt);
                if (rp.Health == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "dead":
            case "notdead":
            {
                if (rp.Dead)
                {
                    result = true;
                }

                break;
            }
            case "weaponfarid":
            case "notweaponfarid":
            {
                _ = int.TryParse(value, out var valueInt);

                if (rp.WeaponGun.Id == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "weaponcacid":
            case "notweaponcacid":
            {
                _ = int.TryParse(value, out var valueInt);

                if (rp.WeaponCac.Id == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "userpvp":
            case "notuserpvp":
            {
                if (rp.PvpEnable)
                {
                    result = true;
                }

                break;
            }
            case "winusermoney":
            case "notwinusermoney":
            {
                result = true;

                foreach (var userSearch in room.RoomUserManager.GetRoomUsers())
                {
                    if (userSearch == null)
                    {
                        continue;
                    }

                    if (userSearch == user)
                    {
                        continue;
                    }

                    if (userSearch.Roleplayer == null)
                    {
                        continue;
                    }

                    if (userSearch.Roleplayer.Money < rp.Money)
                    {
                        continue;
                    }

                    result = false;
                    break;
                }

                break;
            }
        }
        return result;
    }

    private static bool ItemCommand(Room room, Item item, RoomUser user, string effect, string value)
    {
        var result = false;
        switch (effect)
        {
            case "itemcollision":
            case "itemnotcollision":
            {
                if (user == null && item != null)
                {
                    result = true;
                }
                break;
            }
            case "itemissurvive":
            case "itemisnotsurvive":
            {
                var count = 0;

                var itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X - 1, item.Y - 1)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }
                itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X, item.Y - 1)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }
                itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X + 1, item.Y - 1)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }
                itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X - 1, item.Y)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }
                itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X + 1, item.Y)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }
                itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X - 1, item.Y + 1)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }
                itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X, item.Y + 1)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }
                itemTest = room.GameMap.GetCoordinatedItems(new Point(item.X + 1, item.Y + 1)).FirstOrDefault();
                if (itemTest != null && itemTest.ExtraData == "1")
                {
                    count++;
                }

                if (count == 3 || (item.ExtraData == "1" && count == 2))
                {
                    result = true;
                }

                break;
            }
            case "itemmode":
            case "itemnotmode":
            {
                if (int.TryParse(item.ExtraData, out _))
                {
                    if (item.ExtraData == value)
                    {
                        result = true;
                    }
                }
                break;
            }
            case "itemrot":
            case "itemnotrot":
            {
                _ = int.TryParse(value, out var valueInt);

                if (item.Rotation == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "itemdistanceplus":
            {
                if (user == null)
                {
                    break;
                }

                _ = int.TryParse(value, out var valueInt);

                if (Math.Abs(user.X - item.X) >= valueInt && Math.Abs(user.Y - item.Y) >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "itemdistancemoins":
            {
                if (user == null)
                {
                    break;
                }

                _ = int.TryParse(value, out var valueInt);

                if (Math.Abs(user.X - item.X) <= valueInt && Math.Abs(user.Y - item.Y) <= valueInt)
                {
                    result = true;
                }

                break;
            }
        }

        return result;
    }

    private static bool RoomCommand(Room room, string effect, string value)
    {
        var result = false;
        switch (effect)
        {
            case "roomopen":
            case "roomnotopen":
            {
                if (room.RoomData.Access == RoomAccess.Open)
                {
                    result = true;
                }

                break;
            }
            case "roomclose":
            case "roomnotclose":
            {
                if (room.RoomData.Access == RoomAccess.Doorbell)
                {
                    result = true;
                }

                break;
            }
            case "teamallcount":
            case "teamallnotcount":
            {
                var teamManager = room.TeamManager;

                _ = int.TryParse(value, out var count);
                if (teamManager.GetAllPlayer().Count == count)
                {
                    result = true;
                }

                break;
            }
            case "teamredcount":
            case "teamrednotcount":
            {
                var teamManager = room.TeamManager;

                _ = int.TryParse(value, out var count);
                if (teamManager.RedTeam.Count == count)
                {
                    result = true;
                }

                break;
            }
            case "teamyellowcount":
            case "teamyellownotcount":
            {
                var teamManager = room.TeamManager;

                _ = int.TryParse(value, out var count);
                if (teamManager.YellowTeam.Count == count)
                {
                    result = true;
                }

                break;
            }
            case "teambluecount":
            case "teambluenotcount":
            {
                var teamManager = room.TeamManager;

                _ = int.TryParse(value, out var count);
                if (teamManager.BlueTeam.Count == count)
                {
                    result = true;
                }

                break;
            }
            case "teamgreencount":
            case "teamgreennotcount":
            {
                var teamManager = room.TeamManager;

                _ = int.TryParse(value, out var count);
                if (teamManager.GreenTeam.Count == count)
                {
                    result = true;
                }

                break;
            }
        }
        return result;
    }

    private static bool UserCommand(RoomUser user, Room room, string effect, string value)
    {
        var result = false;
        switch (effect)
        {
            case "classement":
            case "notclassement":
            {
                var itemHighScore = room.RoomItemHandling.GetFloor.FirstOrDefault(x => x.GetBaseItem().InteractionType is InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS);
                if (itemHighScore == null)
                {
                    break;
                }

                if (itemHighScore.Scores.TryGetValue(user.GetUsername(), out var score))
                {
                    result = true;
                }

                break;
            }
            case "ptsclassementplus":
            {
                var itemHighScore = room.RoomItemHandling.GetFloor.FirstOrDefault(x => x.GetBaseItem().InteractionType is InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS);
                if (itemHighScore == null)
                {
                    break;
                }

                _ = int.TryParse(value, out var valueInt);

                if (itemHighScore.Scores.TryGetValue(user.GetUsername(), out var score) && score > valueInt)
                {
                    result = true;
                }

                break;
            }
            case "ptsclassementmoins":
            {
                var itemHighScore = room.RoomItemHandling.GetFloor.FirstOrDefault(x => x.GetBaseItem().InteractionType is InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS);
                if (itemHighScore == null)
                {
                    break;
                }

                _ = int.TryParse(value, out var valueInt);

                if (itemHighScore.Scores.TryGetValue(user.GetUsername(), out var score) && score < valueInt)
                {
                    result = true;
                }

                break;
            }
            case "ptsclassement":
            case "notptsclassement":
            {
                var itemHighScore = room.RoomItemHandling.GetFloor.FirstOrDefault(x => x.GetBaseItem().InteractionType is InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS);
                if (itemHighScore == null)
                {
                    break;
                }
                _ = int.TryParse(value, out var valueInt);

                if (itemHighScore.Scores.TryGetValue(user.GetUsername(), out var score) && score == valueInt)
                {
                    result = true;
                }

                break;
            }
            case "winuserpoint":
            case "notwinuserpoint":
            {
                result = true;

                foreach (var userSearch in room.RoomUserManager.GetRoomUsers())
                {
                    if (userSearch == null)
                    {
                        continue;
                    }

                    if (userSearch == user)
                    {
                        continue;
                    }

                    if (userSearch.WiredPoints < user.WiredPoints)
                    {
                        continue;
                    }

                    result = false;
                    break;
                }

                break;
            }
            case "missioncontains":
            case "notmissioncontains":
            {
                if (!user.IsBot && user.Client.User.Motto.Contains(value))
                {
                    result = true;
                }

                break;
            }
            case "mission":
            case "notmission":
            {
                if (!user.IsBot && user.Client.User.Motto == value)
                {
                    result = true;
                }

                break;
            }
            case "favogroupid":
            case "notfavogroupid":
            {
                _ = int.TryParse(value, out var groupId);

                if (!user.IsBot && user.Client.User.FavouriteGroupId == groupId)
                {
                    result = true;
                }

                break;
            }
            case "usergirl":
            case "notusergirl":
            {
                if (!user.IsBot && user.Client.User.Gender.ToUpper() == "F")
                {
                    result = true;
                }

                break;
            }
            case "userboy":
            case "notuserboy":
            {
                if (!user.IsBot && user.Client.User.Gender.ToUpper() == "M")
                {
                    result = true;
                }

                break;
            }
            case "namebot":
            case "notnamebot":
            {
                if (user.IsBot && user.BotData.Name == value)
                {
                    result = true;
                }

                break;
            }
            case "isbot":
            case "notisbot":
            {
                if (user.IsBot)
                {
                    result = true;
                }

                break;
            }
            case "allowshoot":
            case "notallowshoot":
            {
                if (user.AllowShoot)
                {
                    result = true;
                }

                break;
            }
            case "winteam":
            case "notwinteam":
            {
                if (user.Team == TeamType.None)
                {
                    break;
                }

                var winningTeam = room.GameManager.GetWinningTeam();
                if (user.Team == winningTeam)
                {
                    result = true;
                }

                break;
            }
            case "freeze":
            case "notfreeze":
            {
                if (user.Freeze)
                {
                    result = true;
                }

                break;
            }
            case "ingame":
            case "notingame":
            {
                if (user.InGame)
                {
                    result = true;
                }

                break;
            }
            case "usertimer":
            {
                _ = int.TryParse(value, out var points);

                if (user.UserTimer == points)
                {
                    result = true;
                }

                break;
            }
            case "usertimerplus":
            {
                _ = int.TryParse(value, out var points);

                if (user.UserTimer > points)
                {
                    result = true;
                }

                break;
            }
            case "usertimermoins":
            {
                _ = int.TryParse(value, out var point);

                if (user.UserTimer < point)
                {
                    result = true;
                }

                break;
            }
            case "point":
            case "notpoint":
            {
                _ = int.TryParse(value, out var point);

                if (user.WiredPoints == point)
                {
                    result = true;
                }

                break;
            }
            case "pointplus":
            {
                _ = int.TryParse(value, out var points);

                if (user.WiredPoints > points)
                {
                    result = true;
                }

                break;
            }
            case "pointmoins":
            {
                _ = int.TryParse(value, out var points);

                if (user.WiredPoints < points)
                {
                    result = true;
                }

                break;
            }
            case "ingroup":
            case "innotgroup":
            {
                _ = int.TryParse(value, out var groupId);

                if (groupId == 0)
                {
                    break;
                }

                if (user.IsBot || (user.Client != null && user.Client.User != null && user.Client.User.MyGroups.Contains(groupId)))
                {
                    result = true;
                }

                break;
            }
            case "userteam":
            case "usernotteam":
            {
                if (user.Team != TeamType.None)
                {
                    result = true;
                }

                break;
            }
            case "sit":
            case "notsit":
            {
                if (user.ContainStatus("sit"))
                {
                    result = true;
                }

                break;
            }
            case "lay":
            case "notlay":
            {
                if (user.ContainStatus("lay"))
                {
                    result = true;
                }

                break;
            }
            case "rot":
            case "notrot":
            {
                if (user.RotBody.ToString() == value)
                {
                    result = true;
                }

                break;
            }
            case "transf":
            case "nottransf":
            {
                if (user.IsTransf)
                {
                    result = true;
                }

                break;
            }
            case "username":
            case "notusername":
            {
                if (user.GetUsername().ToLower() == value.ToLower())
                {
                    result = true;
                }

                break;
            }
            case "handitem":
            case "nothanditem":
            {
                if (user.CarryItemID.ToString() == value)
                {
                    result = true;
                }

                break;
            }
            case "badge":
            case "notbadge":
            {
                if (user.IsBot || user.Client == null || user.Client.User == null || user.Client.User.BadgeComponent == null)
                {
                    break;
                }

                if (user.Client.User.BadgeComponent.HasBadge(value))
                {
                    result = true;
                }

                break;
            }
            case "enable":
            case "notenable":
            {
                if (user.CurrentEffect.ToString() == value)
                {
                    result = true;
                }

                break;
            }
            case "slotwinner":
            case "notslotwinner":
            {
                if (user.IsSlotWinner)
                {
                    result = true;
                }

                break;
            }
            case "slotspin":
            case "notslotspin":
            {
                if (user.IsSlotSpin)
                {
                    result = true;
                }

                break;
            }
            case "slot":
            case "notslot":
            {
                _ = int.TryParse(value, out var valueInt);

                if (valueInt is <= 0 or > 100)
                {
                    break;
                }

                if (user.Client.User.WibboPoints >= valueInt)
                {
                    result = true;
                }

                break;
            }
            case "rank":
            {
                if (user.IsBot)
                {
                    break;
                }

                if (user.Client.User.Rank.ToString() == value)
                {
                    result = true;
                }

                break;
            }
            case "rankplus":
            {
                if (user.IsBot)
                {
                    break;
                }

                if (user.Client.User.Rank > Convert.ToInt32(value))
                {
                    result = true;
                }

                break;
            }
            case "rankmoin":
            {
                if (user.IsBot)
                {
                    break;
                }

                if (user.Client.User.Rank < Convert.ToInt32(value))
                {
                    result = true;
                }

                break;
            }
        }
        return result;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.StringParam = wiredTriggerData;
}
