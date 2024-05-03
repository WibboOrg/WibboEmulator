namespace WibboEmulator.Games.Items.Wired.Actions;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.RolePlay;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Communication.Packets.Outgoing.Sound;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Database.Daos.User;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Roleplays.Enemy;
using WibboEmulator.Games.Roleplays.Item;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Database;
using WibboEmulator.Core.Settings;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Roleplays.Weapon;
using WibboEmulator.Games.LandingView;
using WibboEmulator.Games.Effects;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Utilities;

public class SuperWired : WiredActionBase, IWired, IWiredEffect
{
    public SuperWired(Item item, Room room) : base(item, room, (int)WiredActionType.CHAT)
    {
    }

    public override void LoadItems(bool inDatabase = false)
    {
        base.LoadItems();

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
            effet = this.StringParam.Split(':')[0];
        }
        else
        {
            effet = this.StringParam;
        }

        switch (effet.ToLower())
        {
            case "rpsendtimeuser":
            case "timespeed":
            case "cyclehoureffect":
            case "setenemy":
            case "enemyaggrostop":
            case "enemyaggrostart":
            case "addenemy":
            case "removeenemy":
            case "userpvp":
            case "pvp":
            case "addmunition":
            case "munition":
            case "rpresetuser":
            case "rpsay":
            case "rpsayme":
            case "rpexp":
            case "rpremoveexp":
            case "removemoney":
            case "addmoney":
            case "work":
            case "health":
            case "healthplus":
            case "hit":
            case "weaponfarid":
            case "weaponcacid":
            case "removeenergy":
            case "addenergy":
            case "removehygiene":
            case "addhygiene":
            case "allowitemsbuy":
            case "inventoryadd":
            case "inventoryremove":
            case "droprpitem":
            case "sendroomid":
            case "botchoose":
            case "botchoosenav":
            case "playsounduser":
            case "playsoundroom":
            case "playmusicroom":
            case "playmusicuser":
            case "stopsounduser":
            case "stopsoundroom":
            case "forcesound":
                if (this.Room.IsRoleplay)
                {
                    return;
                }
                break;
        }

        switch (effet)
        {
            case "botchoosenav":
            case "alert":
            case "send":
            case "enablestaff":
            case "roomingamechat":
            case "jackanddaisy":
            case "openpage":
            case "playsounduser":
            case "playsoundroom":
            case "playmusicroom":
            case "playmusicuser":
            case "stopsounduser":
            case "stopsoundroom":
            case "forcesound":
            case "badge":
            case "removebadge":
            case "roomalert":
            case "rpsay":
            case "rpsayme":
                if (this.IsStaff)
                {
                    return;
                }
                break;

            case "achievement":
            case "givelot":
            case "winmovierun":
            case "givebanner":
            case "startslot":
            case "endslot":
                if (this.IsGod)
                {
                    return;
                }
                break;
        }

        switch (effet)
        {
            case "botchoose":
            case "roomstate":
            case "roomfreeze":
            case "roomkick":
            case "moveto":
            case "arrowmove":
            case "mousemove":
            case "reversewalk":
            case "speedwalk":
            case "configbot":
            case "rot":
            case "roommute":
            case "enable":
            case "dance":
            case "sit":
            case "lay":
            case "handitem":
            case "setspeed":
            case "freeze":
            case "unfreeze":
            case "roomdiagonal":
            case "roomoblique":
            case "point":
            case "usertimer":
            case "addusertimer":
            case "removeusertimer":
            case "addpoint":
            case "removepoint":
            case "ingame":
            case "setitemmode":
            case "useitem":
            case "addpointteam":
            case "breakwalk":
            case "allowshoot":
            case "transf":
            case "transfstop":
            case "pushpull":
            case "stand":
            case "usermute":
            case "resetclassement":
            case "addclassement":
            case "roomgame":
                return;
        }

        this.StringParam = "";
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (string.IsNullOrEmpty(this.StringParam) || this.StringParam == ":")
        {
            return false;
        }

        string command;
        string value;
        if (this.StringParam.Contains(':'))
        {
            command = this.StringParam.Split(':')[0].ToLower();
            value = string.Join(':', this.StringParam.Split(':').Skip(1));
        }
        else
        {
            command = this.StringParam;
            value = string.Empty;
        }

        this.ItemCommand(command, value, item);
        this.RoomCommand(command, value, user);
        this.RpCommand(command, value, user);
        this.UserCommand(command, value, user);
        this.BotCommand(command, value, user);

        return false;
    }

    private void ItemCommand(string command, string value, Item item)
    {
        switch (command)
        {

            case "setitemmode":
            {
                if (item == null)
                {
                    break;
                }

                if (!int.TryParse(value, out var count) || count < 0)
                {
                    break;
                }

                if (item.ItemData.Modes <= 1)
                {
                    break;
                }

                if (count > item.ItemData.Modes - 1)
                {
                    break;
                }

                if (!int.TryParse(item.ExtraData, out _))
                {
                    break;
                }

                item.ExtraData = count.ToString();
                item.UpdateState();
                this.Room.GameMap.UpdateMapForItem(item);

                break;
            }

            case "useitem":
            {
                if (item == null)
                {
                    break;
                }

                if (item.ItemData.Modes <= 1)
                {
                    break;
                }

                if (!int.TryParse(value, out var count) || count <= 0)
                {
                    break;
                }

                if (!int.TryParse(item.ExtraData, out var itemCount))
                {
                    break;
                }

                var newCount = (itemCount + count < item.ItemData.Modes) ? itemCount + count : 0;

                item.ExtraData = newCount.ToString();
                item.UpdateState();
                this.Room.GameMap.UpdateMapForItem(item);

                break;
            }
        }
    }

    private void RpCommand(string command, string value, RoomUser user)
    {
        if (!this.Room.IsRoleplay)
        {
            return;
        }

        if (user == null || user.Client == null)
        {
            return;
        }

        var rp = user.Roleplayer;
        if (rp == null)
        {
            return;
        }

        switch (command)
        {
            case "rpsendtimeuser":
            {
                user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("rp.useitem.showtime", user.Client.Language), this.Room.RoomRoleplay.Hour, this.Room.RoomRoleplay.Minute));
                break;
            }
            case "setenemy":
            {
                var parameters = value.Split(';');
                if (parameters.Length != 3)
                {
                    break;
                }

                var botOrPet = this.Room.RoomUserManager.GetBotOrPetByName(parameters[0]);
                if (botOrPet == null || botOrPet.BotData == null || botOrPet.BotData.RoleBot == null)
                {
                    break;
                }

                RPEnemy rpEnemyConfig;
                if (!botOrPet.IsPet)
                {
                    rpEnemyConfig = RPEnemyManager.GetEnemyBot(botOrPet.BotData.Id);
                }
                else
                {
                    rpEnemyConfig = RPEnemyManager.GetEnemyPet(botOrPet.BotData.Id);
                }

                if (rpEnemyConfig == null)
                {
                    break;
                }

                switch (parameters[1])
                {
                    case "health":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.Health = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateHealth(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "weaponfarid":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.WeaponGunId = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateWeaponFarId(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "weaponcacid":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.WeaponCacId = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateWeaponCacId(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "deadtimer":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.DeadTimer = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateDeadTimer(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "lootitemid":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.LootItemId = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateLootItemId(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "moneydrop":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.MoneyDrop = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateMoneyDrop(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "teamid":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.TeamId = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateTeamId(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "aggrodistance":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.AggroDistance = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateAggroDistance(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "zonedistance":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.ZoneDistance = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateZoneDistance(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "resetposition":
                    {
                        rpEnemyConfig.ResetPosition = parameters[2] == "true";
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateResetPosition(dbClient, rpEnemyConfig.Id, rpEnemyConfig.ResetPosition);

                        break;
                    }
                    case "lostaggrodistance":
                    {
                        if (!int.TryParse(parameters[2], out var paramInt))
                        {
                            break;
                        }

                        if (paramInt <= 0)
                        {
                            paramInt = 0;
                        }

                        if (paramInt > 9999)
                        {
                            paramInt = 9999;
                        }

                        rpEnemyConfig.LostAggroDistance = paramInt;
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateLostAggroDistance(dbClient, rpEnemyConfig.Id, paramInt);

                        break;
                    }
                    case "zombiemode":
                    {
                        rpEnemyConfig.ZombieMode = parameters[2] == "true";
                        botOrPet.BotData.RoleBot.SetConfig(rpEnemyConfig);

                        using var dbClient = DatabaseManager.Connection;
                        RoleplayEnemyDao.UpdateZombieMode(dbClient, rpEnemyConfig.Id, rpEnemyConfig.ZombieMode);

                        break;
                    }
                }
                break;
            }
            case "removeenemy":
            {
                var botOrPet = this.Room.RoomUserManager.GetBotOrPetByName(value);
                if (botOrPet == null || botOrPet.BotData == null || botOrPet.BotData.RoleBot == null)
                {
                    break;
                }

                if (!botOrPet.IsPet)
                {
                    RPEnemyManager.RemoveEnemyBot(botOrPet.BotData.Id);
                    botOrPet.BotData.RoleBot = null;
                    botOrPet.BotData.AiType = BotAIType.Generic;
                    botOrPet.BotAI = botOrPet.BotData.GenerateBotAI(botOrPet.VirtualId);
                    botOrPet.BotAI.Initialize(botOrPet.BotData.Id, botOrPet, botOrPet.Room);
                }
                else
                {
                    RPEnemyManager.RemoveEnemyPet(botOrPet.BotData.Id);
                    botOrPet.BotData.RoleBot = null;
                    botOrPet.BotData.AiType = BotAIType.Pet;
                    botOrPet.BotAI = botOrPet.BotData.GenerateBotAI(botOrPet.VirtualId);
                    botOrPet.BotAI.Initialize(botOrPet.BotData.Id, botOrPet, botOrPet.Room);
                }
                break;
            }
            case "addenemy":
            {
                var botOrPet = this.Room.RoomUserManager.GetBotOrPetByName(value);
                if (botOrPet == null || botOrPet.BotData == null || botOrPet.BotData.RoleBot != null)
                {
                    break;
                }

                if (!botOrPet.IsPet)
                {
                    var rpEnemyConfig = RPEnemyManager.AddEnemyBot(botOrPet.BotData.Id);
                    if (rpEnemyConfig != null)
                    {
                        botOrPet.BotData.RoleBot = new RoleBot(rpEnemyConfig);
                        botOrPet.BotData.AiType = BotAIType.RoleplayBot;
                        botOrPet.BotAI = botOrPet.BotData.GenerateBotAI(botOrPet.VirtualId);
                        botOrPet.BotAI.Initialize(botOrPet.BotData.Id, botOrPet, botOrPet.Room);
                    }
                }
                else
                {
                    var rpEnemyConfig = RPEnemyManager.AddEnemyPet(botOrPet.BotData.Id);
                    if (rpEnemyConfig != null)
                    {
                        botOrPet.BotData.RoleBot = new RoleBot(rpEnemyConfig);
                        botOrPet.BotData.AiType = BotAIType.RoleplayPet;
                        botOrPet.BotAI = botOrPet.BotData.GenerateBotAI(botOrPet.VirtualId);
                        botOrPet.BotAI.Initialize(botOrPet.BotData.Id, botOrPet, botOrPet.Room);
                    }
                }
                break;
            }
            case "enemyaggrostop":
            {
                var botOrPet = this.Room.RoomUserManager.GetBotOrPetByName(value);
                if (botOrPet == null || botOrPet.BotData == null || botOrPet.BotData.RoleBot == null)
                {
                    break;
                }

                botOrPet.BotData.RoleBot.ResetAggro();

                break;
            }
            case "enemyaggrostart":
            {
                var botOrPet = this.Room.RoomUserManager.GetBotOrPetByName(value);
                if (botOrPet == null || botOrPet.BotData == null || botOrPet.BotData.RoleBot == null)
                {
                    break;
                }

                botOrPet.BotData.RoleBot.AggroVirtuelId = user.VirtualId;
                botOrPet.BotData.RoleBot.AggroTimer = 0;

                break;
            }
            case "sendroomid":
            {
                if (int.TryParse(value, out var roomId))
                {
                    var roomDataTarget = RoomManager.GenerateRoomData(roomId);
                    if (roomDataTarget != null && roomDataTarget.OwnerId == this.Room.RoomData.OwnerId && !user.Client.User.IsTeleporting)
                    {
                        this.Room.RoomUserManager.RemoveUserFromRoom(user.Client, true, false);

                        user.Client.User.IsTeleporting = true;
                        user.Client.User.TeleportingRoomID = roomId;
                        user.Client.SendPacket(new RoomForwardComposer(roomId));
                    }
                }
                break;
            }
            case "inventoryadd":
            {
                _ = int.TryParse(value, out var itemId);

                var rpItem = RPItemManager.GetItem(itemId);
                if (rpItem == null)
                {
                    break;
                }

                rp.AddInventoryItem(rpItem.Id);
                break;
            }
            case "inventoryremove":
            {
                _ = int.TryParse(value, out var itemId);

                var rpItem = RPItemManager.GetItem(itemId);
                if (rpItem == null)
                {
                    break;
                }

                rp.RemoveInventoryItem(rpItem.Id);
                break;
            }

            case "rpresetuser":
            {
                rp.Reset();

                break;
            }
            case "userpvp":
            {
                rp.PvpEnable = value == "true";

                break;
            }
            case "userfar":
            {
                rp.FarEnable = value == "true";

                break;
            }
            case "usercac":
            {
                rp.CacEnable = value == "true";

                break;
            }
            case "allowitemsbuy":
            {
                var itemsList = new List<RPItem>();
                user.AllowBuyItems.Clear();

                if (string.IsNullOrEmpty(value))
                {
                    rp.SendItemsList(itemsList);
                    break;
                }

                if (value.Contains(','))
                {
                    foreach (var pId in value.Split(','))
                    {
                        if (!int.TryParse(pId, out var id))
                        {
                            continue;
                        }

                        var rpItem = RPItemManager.GetItem(id);
                        if (rpItem == null)
                        {
                            continue;
                        }

                        itemsList.Add(rpItem);
                        user.AllowBuyItems.Add(id);
                    }
                }
                else
                {
                    if (!int.TryParse(value, out var id))
                    {
                        break;
                    }

                    var rpItem = RPItemManager.GetItem(id);
                    if (rpItem == null)
                    {
                        break;
                    }

                    itemsList.Add(rpItem);
                    user.AllowBuyItems.Add(id);
                }

                rp.SendItemsList(itemsList);

                break;
            }
            case "removeenergy":
            {
                _ = int.TryParse(value, out var count);

                rp.RemoveEnergy(count);

                rp.SendUpdate();
                break;
            }
            case "addenergy":
            {
                _ = int.TryParse(value, out var count);

                rp.AddEnergy(count);

                rp.SendUpdate();
                break;
            }
            case "weaponfarid":
            {
                _ = int.TryParse(value, out var count);
                if (count is < 0 or > 2)
                {
                    count = 0;
                }

                rp.WeaponGun = RPWeaponManager.GetWeaponGun(count);

                break;
            }
            case "weaponcacid":
            {
                _ = int.TryParse(value, out var count);

                if (count is < 0 or > 3)
                {
                    count = 0;
                }

                rp.WeaponCac = RPWeaponManager.GetWeaponCac(count);
                break;
            }
            case "pvp":
            {
                this.Room.RoomRoleplay.Pvp = value == "true";

                break;
            }
            case "munition":
            {
                _ = int.TryParse(value, out var count);
                if (count > 99)
                {
                    count = 99;
                }

                if (count < 0)
                {
                    count = 0;
                }

                rp.Munition = count;

                rp.SendUpdate();
                break;
            }
            case "addmunition":
            {
                _ = int.TryParse(value, out var count);

                rp.AddMunition(count);
                rp.SendUpdate();
                break;
            }
            case "removemunition":
            {
                _ = int.TryParse(value, out var count);

                rp.RemoveMunition(count);
                rp.SendUpdate();
                break;
            }
            case "rpexp":
            {
                _ = int.TryParse(value, out var count);
                if (count <= 0)
                {
                    break;
                }

                rp.AddExp(count);
                break;
            }
            case "rpremoveexp":
            {
                _ = int.TryParse(value, out var count);
                if (count <= 0)
                {
                    break;
                }

                rp.RemoveExp(count);
                break;
            }
            case "removemoney":
            {
                _ = int.TryParse(value, out var count);
                if (count <= 0)
                {
                    break;
                }

                if (rp.Money >= count)
                {
                    rp.Money -= count;
                }
                else
                {
                    rp.Money = 0;
                }

                rp.SendUpdate();
                break;
            }
            case "addmoney":
            {
                _ = int.TryParse(value, out var points);
                if (points <= 0)
                {
                    break;
                }

                if (rp.Money <= int.MaxValue - points)
                {
                    rp.Money += points;
                    rp.SendUpdate();
                }
                else
                {
                    rp.Money = int.MaxValue;
                }

                rp.SendUpdate();
                break;
            }
            case "health":
            {
                _ = int.TryParse(value, out var count);
                if (count <= 0)
                {
                    break;
                }

                if (count > rp.HealthMax)
                {
                    rp.Health = rp.HealthMax;
                }
                else
                {
                    rp.Health = count;
                }

                rp.SendUpdate();
                break;
            }
            case "healthplus":
            {
                _ = int.TryParse(value, out var count);
                if (count <= 0)
                {
                    break;
                }

                rp.AddHealth(count);

                rp.SendUpdate();
                break;
            }
            case "hit":
            {
                _ = int.TryParse(value, out var nb);
                if (nb <= 0)
                {
                    break;
                }

                rp.Hit(user, nb, this.Room, false, true);
                rp.SendUpdate();
                break;
            }
            case "rpsay":
            {
                user.OnChat(value, 0, false);
                break;
            }
            case "rpsayme":
            {
                user.OnChatMe(value, 0, false);
                break;
            }
            case "droprpitem":
            {
                _ = int.TryParse(value, out var valueNumber);
                if (valueNumber <= 0)
                {
                    break;
                }

                _ = this.Room.RoomItemHandling.AddTempItem(user.VirtualId, valueNumber, user.SetX, user.SetY, user.Z, "1", 0, InteractionTypeTemp.RpItem);
                break;
            }
        }
    }

    private void BotCommand(string command, string value, RoomUser user)
    {
        if (user == null || !user.IsBot)
        {
            return;
        }

        switch (command)
        {
            case "dance":
            {
                if (int.TryParse(value, out var danceId))
                {
                    if (danceId is < 0 or > 4)
                    {
                        danceId = 0;
                    }

                    if (danceId > 0 && user.CarryItemID > 0)
                    {
                        user.CarryItem(0);
                    }

                    user.DanceId = danceId;
                    this.Room.SendPacket(new DanceComposer(user.VirtualId, danceId));
                }

                break;
            }

            case "handitem":
            {
                if (int.TryParse(value, out var carryid))
                {
                    user.CarryItem(carryid, true);
                }

                break;
            }
            case "sit":
            {
                if (user.RotBody % 2 == 0)
                {
                    user.SetStatus("sit", "0.5");

                    user.IsSit = true;
                    user.UpdateNeeded = true;
                }
                break;
            }

            case "lay":
            {
                if (user.RotBody % 2 == 0)
                {
                    user.SetStatus("lay", "0.7");

                    user.IsLay = true;
                    user.UpdateNeeded = true;
                }
                break;
            }

            case "stand":
            {
                if (user.ContainStatus("lay"))
                {
                    user.RemoveStatus("lay");
                }

                if (user.ContainStatus("sit"))
                {
                    user.RemoveStatus("sit");
                }

                if (user.ContainStatus("sign"))
                {
                    user.RemoveStatus("sign");
                }

                user.UpdateNeeded = true;
                break;
            }

            case "enable":
            {
                if (!int.TryParse(value, out var numEnable))
                {
                    return;
                }

                if (!EffectManager.HasEffect(numEnable, false))
                {
                    return;
                }

                user.ApplyEffect(numEnable);
                break;
            }

            case "breakwalk":
            {
                user.BreakWalkEnable = value == "true";

                break;
            }

            case "freeze":
            {
                _ = int.TryParse(value, out var seconde);
                seconde *= 2;
                user.Freeze = true;
                user.FreezeEndCounter = seconde;
                break;
            }
            case "unfreeze":
            {
                user.Freeze = false;
                user.FreezeEndCounter = 0;
                break;
            }
        }
    }

    private void RoomCommand(string command, string value, RoomUser user)
    {
        switch (command)
        {
            case "resetclassement":
            {
                var itemHighScore = this.Room.RoomItemHandling.FloorItems.FirstOrDefault(x => x.ItemData.InteractionType is InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS);
                if (itemHighScore == null)
                {
                    break;
                }

                itemHighScore.Scores.Clear();
                itemHighScore.UpdateState(false);
                break;
            }
            case "addclassement":
            {
                var itemHighScore = this.Room.RoomItemHandling.FloorItems.FirstOrDefault(x => x.ItemData.InteractionType is InteractionType.HIGH_SCORE or InteractionType.HIGH_SCORE_POINTS);
                if (itemHighScore == null || user == null)
                {
                    break;
                }

                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                if (itemHighScore.Scores.TryGetValue(user.Username, out var score))
                {
                    itemHighScore.Scores[user.Username] = score + valueInt;
                }
                else
                {
                    itemHighScore.Scores.Add(user.Username, valueInt);
                }

                this.Item.UpdateState(false);

                itemHighScore.UpdateState(false);
                break;
            }

            case "roomfreeze":
            {
                this.Room.FreezeRoom = value == "true";
                break;
            }
            case "roomkick":
            {
                foreach (var rUser in this.Room.RoomUserManager.UserList.ToList())
                {
                    if (rUser != null && rUser.Client != null && !rUser.Client.User.HasPermission("no_kick") && this.Room.RoomData.OwnerId != rUser.UserId)
                    {
                        this.Room.RoomUserManager.RemoveUserFromRoom(rUser.Client, true, false);
                    }
                }
                break;
            }
            case "roomalert":
            {
                if (value.Length <= 0)
                {
                    break;
                }

                foreach (var rUser in this.Room.RoomUserManager.UserList.ToList())
                {
                    if (rUser != null && !rUser.IsBot)
                    {
                        rUser.Client?.SendNotification(value);
                    }
                }
                break;
            }
            case "stopsoundroom":
            {
                this.Room.SendPacket(new StopSoundComposer(value));
                break;
            }
            case "playsoundroom":
            {
                this.Room.SendPacket(new PlaySoundComposer(value, 1)); //Type = Trax
                break;
            }
            case "playmusicroom":
            {
                this.Room.SendPacket(new PlaySoundComposer(value, 2, true)); //Type = Trax
                break;
            }
            case "configbot":
            {
                var parameters = value.Split(';');

                if (parameters.Length < 3)
                {
                    break;
                }

                var bot = this.Room.RoomUserManager.GetBotByName(parameters[0]);
                if (bot == null)
                {
                    return;
                }

                switch (parameters[1])
                {
                    case "enable":
                    {
                        if (parameters.Length < 3)
                        {
                            break;
                        }

                        _ = int.TryParse(parameters[2], out var intValue);

                        if (!EffectManager.HasEffect(intValue, false))
                        {
                            return;
                        }

                        if (bot.CurrentEffect != intValue)
                        {
                            bot.ApplyEffect(intValue);
                        }

                        if (bot.BotData.Enable != intValue)
                        {
                            bot.BotData.Enable = intValue;
                        }

                        break;
                    }
                    case "handitem":
                    {
                        if (parameters.Length < 3)
                        {
                            break;
                        }

                        _ = int.TryParse(parameters[2], out var intValue);

                        if (bot.CarryItemID != intValue)
                        {
                            bot.CarryItem(intValue, true);
                        }

                        if (bot.BotData.Handitem != intValue)
                        {
                            bot.BotData.Handitem = intValue;
                        }

                        break;
                    }
                    case "rot":
                    {
                        if (parameters.Length < 3)
                        {
                            break;
                        }

                        _ = int.TryParse(parameters[2], out var intValue);
                        intValue = (intValue is > 7 or < 0) ? 0 : intValue;

                        if (bot.RotBody != intValue)
                        {
                            bot.RotBody = intValue;
                            bot.RotHead = intValue;
                            bot.UpdateNeeded = true;
                        }

                        if (bot.BotData.Rot != intValue)
                        {
                            bot.BotData.Rot = intValue;
                        }

                        break;
                    }
                    case "sit":
                    {
                        if (bot.BotData.Status == 1)
                        {
                            bot.BotData.Status = 0;

                            bot.RemoveStatus("sit");
                            bot.IsSit = false;
                            bot.UpdateNeeded = true;
                        }
                        else
                        {
                            if (!bot.IsSit)
                            {
                                bot.SetStatus("sit", bot.IsPet ? "" : "0.5");
                                bot.IsSit = true;
                                bot.UpdateNeeded = true;
                            }

                            bot.BotData.Status = 1;
                        }

                        break;
                    }
                    case "lay":
                    {
                        if (bot.BotData.Status == 2)
                        {
                            bot.BotData.Status = 0;

                            bot.RemoveStatus("lay");
                            bot.IsSit = false;
                            bot.UpdateNeeded = true;
                        }
                        else
                        {
                            if (!bot.IsLay)
                            {
                                bot.SetStatus("lay", bot.IsPet ? "" : "0.7");
                                bot.IsLay = true;
                                bot.UpdateNeeded = true;
                            }

                            bot.BotData.Status = 2;
                        }

                        break;
                    }
                }
                break;
            }
            case "timespeed":
            {
                if (!this.Room.IsRoleplay)
                {
                    break;
                }

                this.Room.RoomRoleplay.TimeSpeed = value == "true";

                break;
            }
            case "cyclehoureffect":
            {
                if (!this.Room.IsRoleplay)
                {
                    break;
                }

                this.Room.RoomRoleplay.CycleHourEffect = value == "true";

                break;
            }

            case "jackanddaisy":
            {
                var botName = "Daisy";
                if (WibboEnvironment.GetRandomNumber(0, 1) == 1)
                {
                    botName = "Jack";
                }

                var bot = this.Room.RoomUserManager.GetBotOrPetByName(botName);

                if (bot == null)
                {
                    break;
                }

                var phrases = new List<string>();

                switch (value)
                {
                    case "wait":
                    {
                        phrases.Add("Merci de patienter, le jeu va bientôt commencer !");
                        phrases.Add("Le jeu va commencer dans quelques instants !");
                        phrases.Add("Patience, le jeu débutera sous peu !");
                        phrases.Add("Silence dans la salle, le jeu va débuter !");
                        break;
                    }
                    case "win":
                    {
                        if (bot.BotData.Name == "Jack")
                        {
                            phrases.Add("Fichtre... #username# a gagné !");
                            phrases.Add("Et c'est ce moussaillon de #username# qui repart avec le trésor !");
                            phrases.Add("#username# vient de décrocher une très belle surprise !");
                        }
                        else
                        {
                            phrases.Add("Félicitation à #username# qui remporte la partie !");
                            phrases.Add("Félicitons #username# qui remporte la partie !");
                            phrases.Add("La chance était du côté de #username# aujourd'hui");
                            phrases.Add("#username# est divin!");
                            phrases.Add("#username# est légendaire !");
                        }
                        break;
                    }
                    case "loose":
                    {
                        if (bot.BotData.Name == "Jack")
                        {
                            phrases.Add("Oulà ! #username# vient de se faire botter l'arrière train' !");
                            phrases.Add("#username# rejoint l'équipe des loosers");
                            phrases.Add("Une défaite en bonne et due forme de #username# !");
                        }
                        else
                        {
                            phrases.Add("La prochaine fois tu y arriveras #username#, j'en suis sûre et certain !");
                            phrases.Add("Courage #username#, tu y arriveras la prochaine fois !");
                            phrases.Add("Ne soit pas triste #username#, d'autres occasions se présenteront à toi !");
                        }
                        break;
                    }
                    case "startgame":
                    {
                        phrases.Add("Allons y !");
                        phrases.Add("C'est parti !");
                        phrases.Add("A vos marques, prêts ? Partez !");
                        phrases.Add("Let's go!");
                        phrases.Add("Ne perdons pas plus de temps !");
                        phrases.Add("Que la partie commence !");
                        break;
                    }
                    case "endgame":
                    {
                        phrases.Add("L'animation est terminée, bravo aux gagnants !");
                        phrases.Add("L'animation est enfin terminée ! Reviens nous voir à la prochaine animation !");
                        break;
                    }
                    case "fungame":
                    {
                        if (bot.BotData.Name == "Jack")
                        {
                            phrases.Add("Mhhhh, le niveau n'est pas très haut...");
                            phrases.Add("On sait déjà tous qui sera le grand vaiqueur...");
                            phrases.Add("Qui ne tente rien, n'a rien");
                        }
                        else
                        {
                            phrases.Add("La victoire approche, tenez le coup !");
                            phrases.Add("C'est pour ça qu'il faut toujours avoir un trèfle à 4 feuilles sur soi");
                            phrases.Add("En essayant continuellement, on finit par réussir, plus ça rate, plus on a des chances que ça marque ;)");
                        }
                        break;
                    }
                }

                var textMessage = phrases.GetRandomElement();
                if (user != null)
                {
                    textMessage = textMessage.Replace("#username#", user.Username);
                }

                bot.OnChat(textMessage, 2, true);

                break;
            }
            case "roomingamechat":
            {
                this.Room.IngameChat = value == "true";

                break;
            }
            case "roomstate":
            {
                if (value == "close")
                {
                    this.Room.RoomData.Access = RoomAccess.Doorbell;
                }
                else
                {
                    this.Room.RoomData.Access = RoomAccess.Open;
                }

                break;
            }
            case "roommute":
            {
                this.Room.RoomMuted = value == "true";

                break;
            }
            case "setspeed":
            {
                if (!int.TryParse(value, out var speed))
                {
                    break;
                }

                this.Room.RoomItemHandling.SetSpeed(speed);
                break;
            }
            case "roomdiagonal":
            {
                this.Room.GameMap.DiagonalEnabled = value == "true";

                break;
            }
            case "roomoblique":
            {
                this.Room.GameMap.ObliqueDisable = value == "true";

                break;
            }
            case "pushpull":
            {
                this.Room.PushPullAllowed = value == "true";

                break;
            }
            case "roomgame":
            {
                this.Room.IsGameMode = value == "true";

                break;
            }
        }
    }

    private void UserCommand(string cmd, string value, RoomUser roomUser)
    {
        if (roomUser == null || roomUser.IsBot || roomUser.Client == null)
        {
            return;
        }

        switch (cmd)
        {
            case "usermute":
            {
                if (value == "true")
                {
                    this.Room.AddMute(roomUser.UserId, 24 * 60 * 60);
                }
                else
                {
                    this.Room.RemoveMute(roomUser.UserId);
                }

                break;
            }
            case "botchoosenav":
            {
                var chooseList = new List<string[]>();

                if (string.IsNullOrEmpty(value))
                {
                    roomUser.Client.SendPacket(new BotChooseComposer(chooseList));
                    break;
                }

                foreach (var roomIdString in value.Split(','))
                {
                    if (!int.TryParse(roomIdString, out var roomId))
                    {
                        continue;
                    }

                    var roomData = RoomManager.GenerateRoomData(roomId);

                    if (roomData == null)
                    {
                        continue;
                    }

                    var list = new List<string>
                        {
                            "", //Username
                            "sendroom_" + roomId, //Code
                            $"{roomData.Name} ({roomData.UsersNow}/{roomData.UsersMax})", //Message
                            "" //Look
                        };

                    chooseList.Add(list.ToArray());
                }

                roomUser.Client.SendPacket(new BotChooseComposer(chooseList));

                break;
            }
            case "startslot":
            {
                _ = int.TryParse(value, out var valueInt);

                if (valueInt is <= 0 or > 100)
                {
                    break;
                }

                if (roomUser.Client.User.WibboPoints < valueInt)
                {
                    break;
                }

                roomUser.IsSlot = true;
                roomUser.SlotAmount = valueInt;

                var chooseList = new List<string[]>
                {
                    new string[] { "", "play_slot", string.Format(LanguageManager.TryGetValue("startslot.botchoose", roomUser.Client.Language), roomUser.SlotAmount), "" },
                };

                roomUser.Client.SendPacket(new BotChooseComposer(chooseList));
                break;
            }
            case "endslot":
            {
                roomUser.IsSlotSpin = false;

                if (roomUser.IsSlotWinner)
                {
                    roomUser.SendWhisperChat(string.Format(LanguageManager.TryGetValue("endslot.winner", roomUser.Client.Language), roomUser.SlotAmount), true);
                }
                else
                {
                    roomUser.SendWhisperChat(string.Format(LanguageManager.TryGetValue("endslot.looser", roomUser.Client.Language), roomUser.SlotAmount), true);
                }

                roomUser.Client.SendPacket(new ActivityPointNotificationComposer(roomUser.Client.User.WibboPoints, 0, 105));
                break;
            }
            case "botchoose":
            {
                var chooseList = new List<string[]>();
                if (string.IsNullOrEmpty(value))
                {
                    roomUser.Client.SendPacket(new BotChooseComposer(chooseList));
                    break;
                }

                if (value.Contains(','))
                {
                    foreach (var pChoose in value.Split(','))
                    {
                        var list = pChoose.Split(';').ToList();
                        if (list.Count == 3)
                        {
                            var botOrPet = this.Room.RoomUserManager.GetBotByName(list[0]);
                            if (botOrPet != null && botOrPet.BotData != null)
                            {
                                list.Add(botOrPet.BotData.Look);
                            }
                            else
                            {
                                list.Add("");
                            }

                            chooseList.Add(list.ToArray());
                        }
                    }
                }
                else
                {
                    var list = value.Split(';').ToList();
                    if (list.Count == 3)
                    {
                        var botOrPet = this.Room.RoomUserManager.GetBotByName(list[0]);
                        if (botOrPet != null && botOrPet.BotData != null)
                        {
                            list.Add(botOrPet.BotData.Look);
                        }
                        else
                        {
                            list.Add("");
                        }

                        chooseList.Add(list.ToArray());
                    }
                }

                roomUser.Client.SendPacket(new BotChooseComposer(chooseList));

                break;
            }
            case "stopsounduser":
            case "stopmusicuser":
            {
                roomUser.Client.SendPacket(new StopSoundComposer(value)); //Type = Trax

                break;
            }
            case "playsounduser":
            {
                roomUser.Client.SendPacket(new PlaySoundComposer(value, 1)); //Type = furni

                break;
            }
            case "playmusicuser":
            {
                roomUser.Client.SendPacket(new PlaySoundComposer(value, 2, true)); //Type = Trax

                break;
            }
            case "moveto":
            {
                roomUser.AllowMoveTo = value == "true";

                break;
            }
            case "arrowmove":
            {
                roomUser.AllowArrowMove = value == "true";

                break;
            }
            case "mousemove":
            {
                roomUser.AllowMouseMove = value == "true";

                break;
            }
            case "reversewalk":
            {
                roomUser.ReverseWalk = value == "true";

                break;
            }
            case "speedwalk":
            {
                roomUser.WalkSpeed = value == "true";

                break;
            }
            case "openpage":
            {
                roomUser.Client.SendPacket(new InClientLinkComposer("habbopages/" + value));
                break;
            }
            case "rot":
            {
                _ = int.TryParse(value, out var valueInt);

                if (valueInt is > 7 or < 0)
                {
                    valueInt = 0;
                }

                if (roomUser.RotBody == valueInt && roomUser.RotHead == valueInt)
                {
                    break;
                }

                roomUser.RotBody = valueInt;
                roomUser.RotHead = valueInt;
                roomUser.UpdateNeeded = true;

                break;
            }
            case "stand":
            {
                if (roomUser.ContainStatus("lay"))
                {
                    roomUser.RemoveStatus("lay");
                }

                if (roomUser.ContainStatus("sit"))
                {
                    roomUser.RemoveStatus("sit");
                }

                if (roomUser.ContainStatus("sign"))
                {
                    roomUser.RemoveStatus("sign");
                }

                roomUser.UpdateNeeded = true;
                break;
            }
            case "allowshoot":
            {
                roomUser.AllowShoot = value == "true";

                break;
            }
            case "addpointteam":
            {
                if (roomUser.Team == TeamType.None)
                {
                    break;
                }

                _ = int.TryParse(value, out var count);

                if (this.Room == null)
                {
                    break;
                }

                this.Room.GameManager.AddPointToTeam(roomUser.Team, count, roomUser);
                break;
            }
            case "ingame":
            {
                roomUser.InGame = value == "true";

                break;
            }
            case "usertimer":
            {
                _ = int.TryParse(value, out var points);

                roomUser.UserTimer = points;

                break;
            }
            case "addusertimer":
            {
                _ = int.TryParse(value, out var points);

                if (points == 0)
                {
                    break;
                }

                if (points > 0 && roomUser.UserTimer <= int.MaxValue - points)
                {
                    roomUser.UserTimer += points;
                }

                break;
            }
            case "removeusertimer":
            {
                _ = int.TryParse(value, out var points);

                if (points == 0)
                {
                    break;
                }

                if (points >= roomUser.UserTimer)
                {
                    roomUser.UserTimer = 0;
                }
                else if (points > 0 && roomUser.UserTimer >= int.MinValue + points)
                {
                    roomUser.UserTimer -= points;
                }

                break;
            }
            case "point":
            {
                _ = int.TryParse(value, out var points);

                roomUser.WiredPoints = points;

                break;
            }
            case "addpoint":
            {
                _ = int.TryParse(value, out var points);

                if (points > 0 && roomUser.WiredPoints <= int.MaxValue - points)
                {
                    roomUser.WiredPoints += points;
                }

                break;
            }
            case "removepoint":
            {
                _ = int.TryParse(value, out var points);

                if (points > 0 && roomUser.WiredPoints >= int.MinValue + points)
                {
                    roomUser.WiredPoints -= points;
                }

                break;
            }
            case "freeze":
            {
                _ = int.TryParse(value, out var seconde);
                seconde *= 2;
                roomUser.Freeze = true;
                roomUser.FreezeEndCounter = seconde;
                break;
            }
            case "unfreeze":
            {
                roomUser.Freeze = false;
                roomUser.FreezeEndCounter = 0;
                break;
            }
            case "breakwalk":
            {
                roomUser.BreakWalkEnable = value == "true";

                break;
            }
            case "enable":
            {
                if (!int.TryParse(value, out var numEnable))
                {
                    return;
                }

                if (!EffectManager.HasEffect(numEnable, false))
                {
                    return;
                }

                roomUser.ApplyEffect(numEnable);
                break;
            }
            case "enablestaff":
            {
                if (!int.TryParse(value, out var numEnable))
                {
                    return;
                }

                if (!EffectManager.HasEffect(numEnable, true))
                {
                    return;
                }

                roomUser.ApplyEffect(numEnable);
                break;
            }
            case "dance":
            {
                if (this.Room == null)
                {
                    break;
                }

                if (int.TryParse(value, out var danceId))
                {
                    if (danceId is < 0 or > 4)
                    {
                        danceId = 0;
                    }

                    if (danceId > 0 && roomUser.CarryItemID > 0)
                    {
                        roomUser.CarryItem(0);
                    }

                    roomUser.DanceId = danceId;

                    this.Room.SendPacket(new DanceComposer(roomUser.VirtualId, danceId));
                }
                break;
            }
            case "handitem":
            {
                if (int.TryParse(value, out var carryid))
                {
                    roomUser.CarryItem(carryid, true);
                }

                break;
            }
            case "sit":
            {
                if (roomUser.RotBody % 2 == 0)
                {
                    if (roomUser.IsTransf)
                    {
                        roomUser.SetStatus("sit", "0");
                    }
                    else
                    {
                        roomUser.SetStatus("sit", "0.5");
                    }

                    roomUser.IsSit = true;
                    roomUser.UpdateNeeded = true;
                }
                break;
            }

            case "lay":
            {
                if (roomUser.RotBody % 2 == 0)
                {
                    if (roomUser.IsTransf)
                    {
                        roomUser.SetStatus("lay", "0");
                    }
                    else
                    {
                        roomUser.SetStatus("lay", "0.7");
                    }

                    roomUser.IsLay = true;
                    roomUser.UpdateNeeded = true;
                }
                break;
            }
            case "transf":
            {
                var raceId = 0;
                var petName = value;
                if (value.Contains(' '))
                {
                    if (int.TryParse(value.Split(' ')[1], out raceId))
                    {
                        if (raceId is < 1 or > 50)
                        {
                            raceId = 0;
                        }
                    }

                    petName = value.Split(' ')[0];
                }

                if (roomUser.SetPetTransformation(petName, raceId))
                {
                    roomUser.IsTransf = true;

                    this.Room.SendPacket(new UserRemoveComposer(roomUser.VirtualId));
                    this.Room.SendPacket(new UsersComposer(roomUser));
                }
                break;
            }
            case "transfstop":
            {
                roomUser.IsTransf = false;

                this.Room.SendPacket(new UserRemoveComposer(roomUser.VirtualId));
                this.Room.SendPacket(new UsersComposer(roomUser));
                break;
            }
            case "badge":
            {
                roomUser.Client.User.BadgeComponent.GiveBadge(value);
                break;
            }
            case "removebadge":
            {
                roomUser.Client.User.BadgeComponent.RemoveBadge(value);
                break;
            }

            case "send":
            {
                if (int.TryParse(value, out var roomId) && !roomUser.Client.User.IsTeleporting)
                {
                    this.Room.RoomUserManager.RemoveUserFromRoom(roomUser.Client, true, false);

                    roomUser.Client.User.IsTeleporting = true;
                    roomUser.Client.User.TeleportingRoomID = roomId;
                    roomUser.Client.SendPacket(new RoomForwardComposer(roomId));
                }
                break;
            }
            case "alert":
            {
                roomUser.Client.SendNotification(value);
                break;
            }
            case "achievement":
            {
                _ = AchievementManager.ProgressAchievement(roomUser.Client, value, 1);
                break;
            }
            case "winmovierun":
            {
                if (roomUser.IsBot || roomUser.Client == null || roomUser.Client.User == null || roomUser.Client.User.Rank > 4)
                {
                    break;
                }


                if (roomUser.Username == this.Room.RoomData.OwnerName)
                {
                    break;
                }

                using var dbClient = DatabaseManager.Connection;
                UserDao.UpdateAddRunPoints(dbClient, roomUser.Client.User.Id);

                break;
            }
            case "givebanner":
            {
                if (roomUser.IsBot || roomUser.Client == null || roomUser.Client.User == null || roomUser.Client.User.BannerComponent == null)
                {
                    break;
                }

                if (!int.TryParse(value, out var valueInt))
                {
                    break;
                }

                using var dbClient = DatabaseManager.Connection;
                roomUser.Client.User.BannerComponent.AddBanner(dbClient, valueInt);
                break;
            }
            case "givelot":
            {
                if (roomUser.IsBot || roomUser.Client == null || roomUser.Client.User == null || roomUser.Client.User.Rank > 4)
                {
                    break;
                }

                if (roomUser.Username == this.Room.RoomData.OwnerName)
                {
                    break;
                }

                var allowedOwner = SettingsManager.GetData<string>("givelot.allowed.owner").Split(',');

                if (!allowedOwner.Contains(this.Room.RoomData.OwnerName))
                {
                    break;
                }

                if (roomUser.WiredGivelot)
                {
                    break;
                }

                roomUser.WiredGivelot = true;

                var lootboxId = SettingsManager.GetData<int>("givelot.lootbox.id");

                if (!ItemManager.GetItem(lootboxId, out var itemData))
                {
                    break;
                }

                int nbLot;

                if (roomUser.Client.User.HasPermission("premium_legend"))
                {
                    nbLot = 5;
                }
                else if (roomUser.Client.User.HasPermission("premium_epic"))
                {
                    nbLot = WibboEnvironment.GetRandomNumber(3, 5);
                }
                else if (roomUser.Client.User.HasPermission("premium_classic"))
                {
                    nbLot = WibboEnvironment.GetRandomNumber(2, 3);
                }
                else
                {
                    nbLot = WibboEnvironment.GetRandomNumber(1, 2);
                }

                using var dbClient = DatabaseManager.Connection;

                var items = ItemFactory.CreateMultipleItems(dbClient, itemData, roomUser.Client.User, "", nbLot);

                foreach (var purchasedItem in items)
                {
                    roomUser.Client.User.InventoryComponent.TryAddItem(purchasedItem);
                }

                roomUser.Client.SendNotification(string.Format(LanguageManager.TryGetValue("notif.givelot.sucess", roomUser.Client.Language), nbLot));

                if (this.Room.RoomData.OwnerName == SettingsManager.GetData<string>("autogame.owner"))
                {
                    roomUser.Client.User.GamePointsMonth += 1;
                    HallOfFameManager.UpdateRakings(roomUser.Client.User);
                    UserDao.UpdateAddGamePoints(dbClient, roomUser.Client.User.Id);
                }

                _ = AchievementManager.ProgressAchievement(roomUser.Client, "ACH_Extrabox", 1);
                ModerationManager.LogStaffEntry(1953042, this.Room.RoomData.OwnerName, roomUser.RoomId, string.Empty, "givelot", "SuperWired givelot: " + roomUser.Username);

                break;
            }
        }
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData2, out var delay))
        {
            this.Delay = delay;
        }

        this.StringParam = wiredTriggerData;
    }
}
