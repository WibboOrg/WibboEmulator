using Wibbo.Communication.Packets.Outgoing.Inventory.Furni;
using Wibbo.Communication.Packets.Outgoing.Inventory.Purse;
using Wibbo.Communication.Packets.Outgoing.Notifications;
using Wibbo.Communication.Packets.Outgoing.Rooms.Avatar;
using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Communication.Packets.Outgoing.Users;

using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Roleplay;
using Wibbo.Game.Roleplay.Enemy;
using Wibbo.Game.Roleplay.Player;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;
using Wibbo.Game.Rooms.Games;
using Wibbo.Game.Rooms.AI;
using Wibbo.Communication.Packets.Outgoing.Inventory.Badges;
using Wibbo.Communication.Packets.Outgoing.RolePlay;
using Wibbo.Communication.Packets.Outgoing.Sound.SoundCustom;
using Wibbo.Communication.Packets.Outgoing.Rooms.Session;

namespace Wibbo.Game.Items.Wired.Actions
{
    public class SuperWired : WiredActionBase, IWired, IWiredEffect
    {
        public SuperWired(Item item, Room room) : base(item, room, (int)WiredActionType.CHAT)
        {
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems();

            if(inDatabase)
                return;

            this.CheckPermission();
        }

        private void CheckPermission()
        {
            string effet;
            if (this.StringParam.Contains(":"))
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
                    if (this.RoomInstance.GetWiredHandler().GetRoom().IsRoleplay)
                    {
                        return;
                    }
                    break;
            }

            switch (effet)
            {
                case "botchoosenav":
                case "botchoose":
                case "alert":
                case "send":
                case "enablestaff":
                case "teleportdisabled":
                case "roomingamechat":
                case "jackanddaisy":
                case "openpage":
                case "playsounduser":
                case "playsoundroom":
                case "playmusicroom":
                case "playmusicuser":
                case "stopsounduser":
                case "stopsoundroom":
                case "badge":
                case "removebadge":
                case "roomalert":
                case "forcesound":
                case "coins":
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

            string value = "";


            string command;
            if (this.StringParam.Contains(":"))
            {
                command = this.StringParam.Split(':')[0].ToLower();
                value = this.StringParam.Split(':')[1];
            }
            else
            {
                command = this.StringParam;
            }

            this.RpCommand(command, value, user, item);
            this.UserCommand(command, value, user, item);
            this.RoomCommand(command, value, user, item);
            this.BotCommand(command, value, user, item);

            return false;
        }


        private void RpCommand(string command, string value, RoomUser user, Item item)
        {
            if (!this.RoomInstance.IsRoleplay)
            {
                return;
            }

            if (user == null || user.GetClient() == null)
            {
                return;
            }

            RolePlayer Rp = user.Roleplayer;
            if (Rp == null)
            {
                return;
            }

            switch (command)
            {
                case "rpsendtimeuser":
                    {
                        user.SendWhisperChat("Il est " + this.RoomInstance.Roleplay.Hour + " heures et " + this.RoomInstance.Roleplay.Minute + " minutes");
                        break;
                    }
                case "setenemy":
                    {
                        string[] Params = value.Split(';');
                        if (Params.Length != 3)
                        {
                            break;
                        }

                        RoomUser BotOrPet = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(Params[0]);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                        {
                            break;
                        }

                        RPEnemy RPEnemyConfig;
                        if (!BotOrPet.IsPet)
                        {
                            RPEnemyConfig = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyBot(BotOrPet.BotData.Id);
                        }
                        else
                        {
                            RPEnemyConfig = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyPet(BotOrPet.BotData.Id);
                        }

                        if (RPEnemyConfig == null)
                        {
                            break;
                        }

                        switch (Params[1])
                        {
                            case "health":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.Health = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateHealth(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "weaponfarid":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.WeaponGunId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateWeaponFarId(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "weaponcacid":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.WeaponCacId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateWeaponCacId(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "deadtimer":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.DeadTimer = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateDeadTimer(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "lootitemid":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.LootItemId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateLootItemId(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "moneydrop":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.MoneyDrop = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateMoneyDrop(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "teamid":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.TeamId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateTeamId(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "aggrodistance":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.AggroDistance = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateAggroDistance(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "zonedistance":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.ZoneDistance = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateZoneDistance(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "resetposition":
                                {
                                    RPEnemyConfig.ResetPosition = (Params[2] == "true");
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateResetPosition(dbClient, RPEnemyConfig.Id, RPEnemyConfig.ResetPosition);
                                    }

                                    break;
                                }
                            case "lostaggrodistance":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                    {
                                        break;
                                    }

                                    if (ParamInt <= 0)
                                    {
                                        ParamInt = 0;
                                    }

                                    if (ParamInt > 9999)
                                    {
                                        ParamInt = 9999;
                                    }

                                    RPEnemyConfig.LostAggroDistance = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateLostAggroDistance(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "zombiemode":
                                {
                                    RPEnemyConfig.ZombieMode = (Params[2] == "true");
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateZombieMode(dbClient, RPEnemyConfig.Id, RPEnemyConfig.ZombieMode);
                                    }

                                    break;
                                }
                        }
                        break;
                    }
                case "removeenemy":
                    {
                        RoomUser BotOrPet = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                        {
                            break;
                        }

                        if (!BotOrPet.IsPet)
                        {
                            WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().RemoveEnemyBot(BotOrPet.BotData.Id);
                            BotOrPet.BotData.RoleBot = null;
                            BotOrPet.BotData.AiType = BotAIType.Generic;
                            BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                        }
                        else
                        {
                            WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().RemoveEnemyPet(BotOrPet.BotData.Id);
                            BotOrPet.BotData.RoleBot = null;
                            BotOrPet.BotData.AiType = BotAIType.Pet;
                            BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                        }
                        break;
                    }
                case "addenemy":
                    {
                        RoomUser BotOrPet = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot != null)
                        {
                            break;
                        }

                        if (!BotOrPet.IsPet)
                        {
                            RPEnemy RPEnemyConfig = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().AddEnemyBot(BotOrPet.BotData.Id);
                            if (RPEnemyConfig != null)
                            {
                                BotOrPet.BotData.RoleBot = new RoleBot(RPEnemyConfig);
                                BotOrPet.BotData.AiType = BotAIType.RoleplayBot;
                                BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                            }
                        }
                        else
                        {
                            RPEnemy RPEnemyConfig = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().AddEnemyPet(BotOrPet.BotData.Id);
                            if (RPEnemyConfig != null)
                            {
                                BotOrPet.BotData.RoleBot = new RoleBot(RPEnemyConfig);
                                BotOrPet.BotData.AiType = BotAIType.RoleplayPet;
                                BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                            }
                        }
                        break;
                    }
                case "enemyaggrostop":
                    {
                        RoomUser BotOrPet = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                        {
                            break;
                        }

                        BotOrPet.BotData.RoleBot.ResetAggro();

                        break;
                    }
                case "enemyaggrostart":
                    {
                        RoomUser BotOrPet = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                        {
                            break;
                        }

                        BotOrPet.BotData.RoleBot.AggroVirtuelId = user.VirtualId;
                        BotOrPet.BotData.RoleBot.AggroTimer = 0;

                        break;
                    }
                case "sendroomid":
                    {
                        if (int.TryParse(value, out int RoomId))
                        {
                            Room roomTarget = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                            if (roomTarget != null && roomTarget.RoomData.OwnerId == this.RoomInstance.RoomData.OwnerId)
                            {
                                user.GetClient().GetUser().IsTeleporting = true;
                                user.GetClient().GetUser().TeleportingRoomID = RoomId;
                                user.GetClient().SendPacket(new RoomForwardComposer(RoomId));
                            }
                        }
                        break;
                    }
                case "inventoryadd":
                    {
                        int.TryParse(value, out int ItemId);

                        RPItem RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
                        if (RpItem == null)
                        {
                            break;
                        }

                        Rp.AddInventoryItem(RpItem.Id);
                        break;
                    }
                case "inventoryremove":
                    {
                        int.TryParse(value, out int ItemId);

                        RPItem RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
                        if (RpItem == null)
                        {
                            break;
                        }

                        Rp.RemoveInventoryItem(RpItem.Id);
                        break;
                    }

                case "rpresetuser":
                    {
                        Rp.Reset();

                        break;
                    }
                case "userpvp":
                    {
                        if (value == "true")
                        {
                            Rp.PvpEnable = true;
                        }
                        else
                        {
                            Rp.PvpEnable = false;
                        }

                        break;
                    }
                case "allowitemsbuy":
                    {
                        List<RPItem> ItemsList = new List<RPItem>();
                        user.AllowBuyItems.Clear();

                        if (string.IsNullOrEmpty(value))
                        {
                            Rp.SendItemsList(ItemsList);
                            break;
                        }

                        if (value.Contains(","))
                        {
                            foreach (string pId in value.Split(','))
                            {
                                if (!int.TryParse(pId, out int Id))
                                {
                                    continue;
                                }

                                RPItem RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Id);
                                if (RpItem == null)
                                {
                                    continue;
                                }

                                ItemsList.Add(RpItem);
                                user.AllowBuyItems.Add(Id);
                            }
                        }
                        else
                        {
                            if (!int.TryParse(value, out int Id))
                            {
                                break;
                            }

                            RPItem RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Id);
                            if (RpItem == null)
                            {
                                break;
                            }

                            ItemsList.Add(RpItem);
                            user.AllowBuyItems.Add(Id);
                        }

                        Rp.SendItemsList(ItemsList);

                        break;
                    }
                case "removeenergy":
                    {
                        int.TryParse(value, out int Nb);

                        Rp.RemoveEnergy(Nb);

                        Rp.SendUpdate();
                        break;
                    }
                case "addenergy":
                    {
                        int.TryParse(value, out int Nb);

                        Rp.AddEnergy(Nb);

                        Rp.SendUpdate();
                        break;
                    }
                case "weaponfarid":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb < 0 || Nb > 2)
                        {
                            Nb = 0;
                        }

                        Rp.WeaponGun = WibboEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(Nb);

                        break;
                    }
                case "weaponcacid":
                    {
                        int.TryParse(value, out int Nb);

                        if (Nb < 0 || Nb > 3)
                        {
                            Nb = 0;
                        }

                        Rp.WeaponCac = WibboEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(Nb);
                        break;
                    }
                case "pvp":
                    {
                        if (value == "true")
                        {
                            this.RoomInstance.Roleplay.Pvp = true;
                        }
                        else
                        {
                            this.RoomInstance.Roleplay.Pvp = false;
                        }

                        break;
                    }
                case "munition":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb > 99)
                        {
                            Nb = 99;
                        }

                        if (Nb < 0)
                        {
                            Nb = 0;
                        }

                        Rp.Munition = Nb;

                        Rp.SendUpdate();
                        break;
                    }
                case "addmunition":
                    {
                        int.TryParse(value, out int Nb);

                        Rp.AddMunition(Nb);
                        Rp.SendUpdate();
                        break;
                    }
                case "removemunition":
                    {
                        int.TryParse(value, out int Nb);

                        Rp.RemoveMunition(Nb);
                        Rp.SendUpdate();
                        break;
                    }
                case "rpexp":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.AddExp(Nb);
                        break;
                    }
                case "rpremoveexp":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.RemoveExp(Nb);
                        break;
                    }
                case "removemoney":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        if (Rp.Money - Nb < 0)
                        {
                            Rp.Money = 0;
                        }
                        else
                        {
                            Rp.Money -= Nb;
                        }
                        Rp.SendUpdate();
                        break;
                    }
                case "addmoney":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.Money += Nb;
                        Rp.SendUpdate();
                        break;
                    }
                case "health":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        if (Nb > Rp.HealthMax)
                        {
                            Rp.Health = Rp.HealthMax;
                        }
                        else
                        {
                            Rp.Health = Nb;
                        }

                        Rp.SendUpdate();
                        break;
                    }
                case "healthplus":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.AddHealth(Nb);

                        Rp.SendUpdate();
                        break;
                    }
                case "hit":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.Hit(user, Nb, this.RoomInstance, false, true);
                        Rp.SendUpdate();
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
                        int.TryParse(value, out int ValueNumber);
                        if (ValueNumber <= 0)
                        {
                            break;
                        }

                        this.RoomInstance.GetRoomItemHandler().AddTempItem(user.VirtualId, ValueNumber, user.SetX, user.SetY, user.Z, "1", 0, InteractionTypeTemp.RPITEM);
                        break;
                    }
            }
        }

        private void BotCommand(string command, string value, RoomUser user, Item item)
        {
            if (user == null || !user.IsBot)
            {
                return;
            }

            switch (command)
            {
                case "dance":
                    {
                        if (int.TryParse(value, out int danceId))
                        {
                            if (danceId < 0 || danceId > 4)
                            {
                                danceId = 0;
                            }

                            if (danceId > 0 && user.CarryItemID > 0)
                            {
                                user.CarryItem(0);
                            }

                            user.DanceId = danceId;
                            user.Room.SendPacket(new DanceComposer(user.VirtualId, danceId));
                        }

                        break;
                    }

                case "handitem":
                    {
                        if (int.TryParse(value, out int carryid))
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
                        if (user.Statusses.ContainsKey("lay"))
                        {
                            user.RemoveStatus("lay");
                        }

                        if (user.Statusses.ContainsKey("sit"))
                        {
                            user.RemoveStatus("sit");
                        }

                        if (user.Statusses.ContainsKey("sign"))
                        {
                            user.RemoveStatus("sign");
                        }

                        user.UpdateNeeded = true;
                        break;
                    }

                case "enable":
                    {
                        if (!int.TryParse(value, out int NumEnable))
                        {
                            return;
                        }

                        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, false))
                        {
                            return;
                        }

                        user.ApplyEffect(NumEnable);
                        break;
                    }

                case "breakwalk":
                    {
                        if (value == "true")
                        {
                            user.BreakWalkEnable = true;
                        }
                        else
                        {
                            user.BreakWalkEnable = false;
                        }

                        break;
                    }

                case "freeze":
                    {
                        int.TryParse(value, out int Seconde);
                        Seconde *= 2;
                        user.Freeze = true;
                        user.FreezeEndCounter = Seconde;
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

        private void RoomCommand(string command, string value, RoomUser user, Item item)
        {
            switch (command)
            {
                case "roomfreeze":
                    {
                        this.RoomInstance.FreezeRoom = (value == "true") ? true : false;
                        break;
                    }
                case "roomkick":
                    {
                        foreach (RoomUser RUser in this.RoomInstance.GetRoomUserManager().GetUserList().ToList())
                        {
                            if (RUser != null && !RUser.IsBot && !RUser.GetClient().GetUser().HasFuse("fuse_no_kick") && this.RoomInstance.RoomData.OwnerId != RUser.UserId)
                            {
                                this.RoomInstance.GetRoomUserManager().RemoveUserFromRoom(RUser.GetClient(), true, false);
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

                        foreach (RoomUser RUser in this.RoomInstance.GetRoomUserManager().GetUserList().ToList())
                        {
                            if (RUser != null && !RUser.IsBot && !RUser.GetClient().GetUser().HasFuse("fuse_no_kick"))
                            {
                                RUser.GetClient().SendNotification(value);
                            }
                        }
                        break;
                    }
                case "stopsoundroom":
                    {
                        this.RoomInstance.SendPacket(new StopSoundComposer(value));
                        break;
                    }
                case "playsoundroom":
                    {
                        this.RoomInstance.SendPacket(new PlaySoundComposer(value, 1)); //Type = Trax
                        break;
                    }
                case "playmusicroom":
                    {
                        this.RoomInstance.SendPacket(new PlaySoundComposer(value, 2, true)); //Type = Trax
                        break;
                    }
                case "configbot":
                    {
                        string[] Params = value.Split(';');

                        RoomUser Bot = this.RoomInstance.GetRoomUserManager().GetBotByName(Params[0]);
                        if (Bot == null)
                        {
                            return;
                        }

                        switch (Params[1])
                        {
                            case "enable":
                                {
                                    if (Params.Length < 3)
                                    {
                                        break;
                                    }

                                    int.TryParse(Params[2], out int IntValue);

                                    if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(IntValue, false))
                                    {
                                        return;
                                    }

                                    if (Bot.CurrentEffect != IntValue)
                                    {
                                        Bot.ApplyEffect(IntValue);
                                    }

                                    if (Bot.BotData.Enable != IntValue)
                                    {
                                        Bot.BotData.Enable = IntValue;
                                    }

                                    break;
                                }
                            case "handitem":
                                {
                                    if (Params.Length < 3)
                                    {
                                        break;
                                    }

                                    int.TryParse(Params[2], out int IntValue);

                                    if (Bot.CarryItemID != IntValue)
                                    {
                                        Bot.CarryItem(IntValue, true);
                                    }

                                    if (Bot.BotData.Handitem != IntValue)
                                    {
                                        Bot.BotData.Handitem = IntValue;
                                    }

                                    break;
                                }
                            case "rot":
                                {
                                    if (Params.Length < 3)
                                    {
                                        break;
                                    }

                                    int.TryParse(Params[2], out int IntValue);
                                    IntValue = (IntValue > 7 || IntValue < 0) ? 0 : IntValue;

                                    if (Bot.RotBody != IntValue)
                                    {
                                        Bot.RotBody = IntValue;
                                        Bot.RotHead = IntValue;
                                        Bot.UpdateNeeded = true;
                                    }

                                    if (Bot.BotData.Rot != IntValue)
                                    {
                                        Bot.BotData.Rot = IntValue;
                                    }

                                    break;
                                }
                            case "sit":
                                {
                                    if (Bot.BotData.Status == 1)
                                    {
                                        Bot.BotData.Status = 0;

                                        Bot.RemoveStatus("sit");
                                        Bot.IsSit = false;
                                        Bot.UpdateNeeded = true;
                                    }
                                    else
                                    {
                                        if (!Bot.IsSit)
                                        {
                                            Bot.SetStatus("sit", (Bot.IsPet) ? "" : "0.5");
                                            Bot.IsSit = true;
                                            Bot.UpdateNeeded = true;
                                        }

                                        Bot.BotData.Status = 1;
                                    }

                                    break;
                                }
                            case "lay":
                                {
                                    if (Bot.BotData.Status == 2)
                                    {
                                        Bot.BotData.Status = 0;

                                        Bot.RemoveStatus("lay");
                                        Bot.IsSit = false;
                                        Bot.UpdateNeeded = true;
                                    }
                                    else
                                    {
                                        if (!Bot.IsLay)
                                        {
                                            Bot.SetStatus("lay", (Bot.IsPet) ? "" : "0.7");
                                            Bot.IsLay = true;
                                            Bot.UpdateNeeded = true;
                                        }

                                        Bot.BotData.Status = 2;
                                    }

                                    break;
                                }
                        }
                        break;
                    }
                case "TimeSpeed":
                    {
                        if (!this.RoomInstance.IsRoleplay)
                        {
                            break;
                        }

                        if (value == "true")
                        {
                            this.RoomInstance.Roleplay.TimeSpeed = true;
                        }
                        else
                        {
                            this.RoomInstance.Roleplay.TimeSpeed = false;
                        }

                        break;
                    }
                case "cyclehoureffect":
                    {
                        if (!this.RoomInstance.IsRoleplay)
                        {
                            break;
                        }

                        if (value == "true")
                        {
                            this.RoomInstance.Roleplay.CycleHourEffect = true;
                        }
                        else
                        {
                            this.RoomInstance.Roleplay.CycleHourEffect = false;
                        }

                        break;
                    }
                
                case "jackanddaisy":
                    {
                        RoomUser Bot;
                        if (WibboEnvironment.GetRandomNumber(0, 1) == 1)
                        {
                            Bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName("Jack");
                        }
                        else
                        {
                            Bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName("Daisy");
                        }

                        if (Bot == null)
                        {
                            break;
                        }

                        List<string> Phrases = new List<string>();

                        switch (value)
                        {
                            case "wait":
                                {


                                    Phrases.Add("Merci de patienter, le jeu va bientôt commencer !");
                                    Phrases.Add("Le jeu va commencer dans quelques instants !");
                                    Phrases.Add("Patience, le jeu débutera sous peu !");
                                    Phrases.Add("Silence dans la salle, le jeu va débuter !");
                                    break;
                                }
                            case "win":
                                {
                                    if (Bot.BotData.Name == "Jack")
                                    {
                                        Phrases.Add("Fichtre... #username# a gagné !");
                                        Phrases.Add("Et c'est ce moussaillon de #username# qui repart avec le trésor !");
                                        Phrases.Add("#username# vient de décrocher une très belle surprise !");
                                    }
                                    else
                                    {
                                        Phrases.Add("Félicitation à #username# qui remporte la partie !");
                                        Phrases.Add("Félicitons #username# qui remporte la partie !");
                                        Phrases.Add("La chance était du côté de #username# aujourd'hui");
                                        Phrases.Add("#username# est divin!");
                                        Phrases.Add("#username# est légendaire !");
                                    }
                                    break;
                                }
                            case "loose":
                                {
                                    if (Bot.BotData.Name == "Jack")
                                    {
                                        Phrases.Add("Oulà ! #username# vient de se faire botter l'arrière train' !");
                                        Phrases.Add("#username# rejoint l'équipe des loosers");
                                        Phrases.Add("Une défaite en bonne et due forme de #username# !");
                                    }
                                    else
                                    {
                                        Phrases.Add("La prochaine fois tu y arriveras #username#, j'en suis sûre et certain !");
                                        Phrases.Add("Courage #username#, tu y arriveras la prochaine fois !");
                                        Phrases.Add("Ne soit pas triste #username#, d'autres occasions se présenteront à toi !");
                                    }
                                    break;
                                }
                            case "startgame":
                                {
                                    Phrases.Add("Allons y !");
                                    Phrases.Add("C'est parti !");
                                    Phrases.Add("A vos marques, prêts ? Partez !");
                                    Phrases.Add("Let's go!");
                                    Phrases.Add("Ne perdons pas plus de temps !");
                                    Phrases.Add("Que la partie commence !");
                                    break;
                                }
                            case "endgame":
                                {
                                    Phrases.Add("L'animation est terminée, bravo aux gagnants !");
                                    Phrases.Add("L'animation est enfin terminée ! Reviens nous voir à la prochaine animation !");
                                    break;
                                }
                            case "fungame":
                                {
                                    if (Bot.BotData.Name == "Jack")
                                    {
                                        Phrases.Add("Mhhhh, le niveau n'est pas très haut...");
                                        Phrases.Add("On sait déjà tous qui sera le grand vaiqueur...");
                                        Phrases.Add("Qui ne tente rien, n'a rien");
                                    }
                                    else
                                    {
                                        Phrases.Add("La victoire approche, tenez le coup !");
                                        Phrases.Add("C'est pour ça qu'il faut toujours avoir un trèfle à 4 feuilles sur soi");
                                        Phrases.Add("En essayant continuellement, on finit par réussir, plus ça rate, plus on a des chances que ça marque ;)");
                                    }
                                    break;
                                }
                        }

                        string TextMessage = Phrases[WibboEnvironment.GetRandomNumber(0, Phrases.Count - 1)];
                        if (user != null)
                        {
                            TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                        }

                        Bot.OnChat(TextMessage, 2, true);

                        break;
                    }
                case "roomingamechat":
                    {
                        if (value == "true")
                        {
                            this.RoomInstance.RoomIngameChat = true;
                        }
                        else
                        {
                            this.RoomInstance.RoomIngameChat = false;
                        }

                        break;
                    }
                case "roomstate":
                    {
                        if (value == "close")
                        {
                            this.RoomInstance.RoomData.State = 1;
                        }
                        else
                        {
                            this.RoomInstance.RoomData.State = 0;
                        }

                        break;
                    }
                case "roommute":
                    {
                        if (value == "true")
                        {
                            this.RoomInstance.RoomMuted = true;
                        }
                        else
                        {
                            this.RoomInstance.RoomMuted = false;
                        }

                        break;
                    }
                case "setspeed":
                    {
                        int.TryParse(value, out int Vitesse);

                        this.RoomInstance.GetRoomItemHandler().SetSpeed(Vitesse);
                        break;
                    }
                case "roomdiagonal":
                    {
                        if (value == "true")
                        {
                            this.RoomInstance.GetGameMap().DiagonalEnabled = true;
                        }
                        else
                        {
                            this.RoomInstance.GetGameMap().DiagonalEnabled = false;
                        }

                        break;
                    }
                case "roomoblique":
                    {
                        if (value == "true")
                        {
                            this.RoomInstance.GetGameMap().ObliqueDisable = true;
                        }
                        else
                        {
                            this.RoomInstance.GetGameMap().ObliqueDisable = false;
                        }

                        break;
                    }

                case "setitemmode":
                    {
                        if (item == null)
                        {
                            break;
                        }

                        int.TryParse(value, out int Count);

                        if (Count > item.GetBaseItem().Modes - 1)
                        {
                            break;
                        }

                        if (!int.TryParse(item.ExtraData, out int result))
                        {
                            break;
                        }

                        item.ExtraData = Count.ToString();
                        item.UpdateState();
                        this.RoomInstance.GetGameMap().UpdateMapForItem(item);

                        break;
                    }

                case "useitem":
                    {
                        if (item == null)
                        {
                            break;
                        }

                        if (item.GetBaseItem().Modes == 0)
                        {
                            break;
                        }

                        int.TryParse(value, out int Count);

                        if (!int.TryParse(item.ExtraData, out int ItemCount))
                        {
                            break;
                        }

                        int newCount = (ItemCount + Count < item.GetBaseItem().Modes) ? ItemCount + Count : 0;

                        item.ExtraData = newCount.ToString();
                        item.UpdateState();
                        this.RoomInstance.GetGameMap().UpdateMapForItem(item);

                        break;
                    }


                case "pushpull":
                    {
                        if (value == "true")
                        {
                            this.RoomInstance.PushPullAllowed = true;
                        }
                        else
                        {
                            this.RoomInstance.PushPullAllowed = false;
                        }

                        break;
                    }
            }
        }

        private void UserCommand(string Cmd, string Value, RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
            {
                return;
            }

            switch (Cmd)
            {
                case "usermute":
                    {
                        if(Value == "true")
                            user.Room.AddMute(user.UserId, 24 * 60 * 60);
                        else
                            user.Room.RemoveMute(user.UserId);
                        break;
                    }
                case "botchoosenav":
                    {
                        List<string[]> ChooseList = new List<string[]>();

                        if (string.IsNullOrEmpty(Value))
                        {
                            user.GetClient().SendPacket(new BotChooseComposer(ChooseList));
                            break;
                        }

                        foreach (string RoomIdString in Value.Split(','))
                        {
                            if (!int.TryParse(RoomIdString, out int RoomId))
                            {
                                continue;
                            }

                            RoomData RoomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

                            if (RoomData == null)
                            {
                                continue;
                            }

                            List<string> list = new List<string>
                            {
                                "", //Username
                                "sendroom_" + RoomId, //Code
                                $"{RoomData.Name} ({RoomData.UsersNow}/{RoomData.UsersMax})", //Message
                                "" //Look
                            };

                            ChooseList.Add(list.ToArray());
                        }

                        user.GetClient().SendPacket(new BotChooseComposer(ChooseList));

                        break;
                    }
                case "botchoose":
                    {
                        List<string[]> ChooseList = new List<string[]>();
                        if (string.IsNullOrEmpty(Value))
                        {
                            user.GetClient().SendPacket(new BotChooseComposer(ChooseList));
                            break;
                        }

                        if (Value.Contains(","))
                        {
                            foreach (string pChoose in Value.Split(','))
                            {
                                List<string> list = pChoose.Split(';').ToList();
                                if (list.Count == 3)
                                {
                                    RoomUser BotOrPet = user.Room.GetRoomUserManager().GetBotByName(list[0]);
                                    if (BotOrPet != null && BotOrPet.BotData != null)
                                    {
                                        list.Add(BotOrPet.BotData.Look);
                                    }
                                    else
                                    {
                                        list.Add("");
                                    }

                                    ChooseList.Add(list.ToArray());
                                }
                            }
                        }
                        else
                        {
                            List<string> list = Value.Split(';').ToList();
                            if (list.Count == 3)
                            {
                                RoomUser BotOrPet = user.Room.GetRoomUserManager().GetBotByName(list[0]);
                                if (BotOrPet != null && BotOrPet.BotData != null)
                                {
                                    list.Add(BotOrPet.BotData.Look);
                                }
                                else
                                {
                                    list.Add("");
                                }

                                ChooseList.Add(list.ToArray());
                            }
                        }

                        user.GetClient().SendPacket(new BotChooseComposer(ChooseList));

                        break;
                    }
                case "stopsounduser":
                    {
                        user.GetClient().SendPacket(new StopSoundComposer(Value)); //Type = Trax

                        break;
                    }
                case "playsounduser":
                    {
                        user.GetClient().SendPacket(new PlaySoundComposer(Value, 1)); //Type = furni

                        break;
                    }
                case "playmusicuser":
                    {
                        user.GetClient().SendPacket(new PlaySoundComposer(Value, 2, true)); //Type = Trax

                        break;
                    }
                case "moveto":
                    {
                        if (Value == "true")
                        {
                            user.AllowMoveTo = true;
                        }
                        else
                        {
                            user.AllowMoveTo = false;
                        }

                        break;
                    }
                case "reversewalk":
                    {
                        if (Value == "true")
                        {
                            user.ReverseWalk = true;
                        }
                        else
                        {
                            user.ReverseWalk = false;
                        }

                        break;
                    }
                case "speedwalk":
                    {
                        if (Value == "true")
                        {
                            user.WalkSpeed = true;
                        }
                        else
                        {
                            user.WalkSpeed = false;
                        }

                        break;
                    }
                case "openpage":
                    {
                        user.GetClient().SendPacket(new InClientLinkComposer("habbopages/" + Value));
                        break;
                    }
                case "rot":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (ValueInt > 7 || ValueInt < 0)
                        {
                            ValueInt = 0;
                        }

                        if (user.RotBody == ValueInt && user.RotHead == ValueInt)
                        {
                            break;
                        }

                        user.RotBody = ValueInt;
                        user.RotHead = ValueInt;
                        user.UpdateNeeded = true;

                        break;
                    }
                case "stand":
                    {
                        if (user.Statusses.ContainsKey("lay"))
                        {
                            user.RemoveStatus("lay");
                        }

                        if (user.Statusses.ContainsKey("sit"))
                        {
                            user.RemoveStatus("sit");
                        }

                        if (user.Statusses.ContainsKey("sign"))
                        {
                            user.RemoveStatus("sign");
                        }

                        user.UpdateNeeded = true;
                        break;
                    }
                case "allowshoot":
                    {
                        if (Value == "true")
                        {
                            user.AllowShoot = true;
                        }
                        else
                        {
                            user.AllowShoot = false;
                        }

                        break;
                    }
                case "addpointteam":
                    {
                        if (user.Team == TeamType.NONE)
                        {
                            break;
                        }

                        int.TryParse(Value, out int Count);

                        if (user.Room == null)
                        {
                            break;
                        }

                        user.Room.GetGameManager().AddPointToTeam(user.Team, Count, user);
                        break;
                    }
                case "ingame":
                    {
                        if (Value == "true")
                        {
                            user.InGame = true;
                        }
                        else
                        {
                            user.InGame = false;
                        }

                        break;
                    }
                case "usertimer":
                    {
                        int.TryParse(Value, out int Points);

                        user.UserTimer = Points;

                        break;
                    }
                case "addusertimer":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        user.UserTimer += Points;

                        break;
                    }
                case "removeusertimer":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        if (Points >= user.UserTimer)
                        {
                            user.UserTimer = 0;
                        }
                        else
                        {
                            user.UserTimer -= Points;
                        }

                        break;
                    }
                case "point":
                    {
                        int.TryParse(Value, out int Points);

                        user.WiredPoints = Points;

                        break;
                    }
                case "addpoint":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        user.WiredPoints += Points;

                        break;
                    }
                case "removepoint":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        if (Points >= user.WiredPoints)
                        {
                            user.WiredPoints = 0;
                        }
                        else
                        {
                            user.WiredPoints -= Points;
                        }
                        break;
                    }
                case "freeze":
                    {
                        int.TryParse(Value, out int Seconde);
                        Seconde *= 2;
                        user.Freeze = true;
                        user.FreezeEndCounter = Seconde;
                        break;
                    }
                case "unfreeze":
                    {
                        user.Freeze = false;
                        user.FreezeEndCounter = 0;
                        break;
                    }
                case "breakwalk":
                    {
                        if (Value == "true")
                        {
                            user.BreakWalkEnable = true;
                        }
                        else
                        {
                            user.BreakWalkEnable = false;
                        }

                        break;
                    }
                case "enable":
                    {
                        if (!int.TryParse(Value, out int NumEnable))
                        {
                            return;
                        }

                        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, false))
                        {
                            return;
                        }

                        user.ApplyEffect(NumEnable);
                        break;
                    }
                case "enablestaff":
                    {
                        if (!int.TryParse(Value, out int NumEnable))
                        {
                            return;
                        }

                        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, true))
                        {
                            return;
                        }

                        user.ApplyEffect(NumEnable);
                        break;
                    }
                case "dance":
                    {
                        if (user.Room == null)
                        {
                            break;
                        }

                        if (int.TryParse(Value, out int danceId))
                        {
                            if (danceId < 0 || danceId > 4)
                            {
                                danceId = 0;
                            }

                            if (danceId > 0 && user.CarryItemID > 0)
                            {
                                user.CarryItem(0);
                            }

                            user.DanceId = danceId;

                            user.Room.SendPacket(new DanceComposer(user.VirtualId, danceId));
                        }
                        break;
                    }
                case "handitem":
                    {
                        if (int.TryParse(Value, out int carryid))
                        {
                            user.CarryItem(carryid, true);
                        }

                        break;
                    }
                case "sit":
                    {
                        if (user.RotBody % 2 == 0)
                        {
                            if (user.IsTransf)
                            {
                                user.SetStatus("sit", "");
                            }
                            else
                            {
                                user.SetStatus("sit", "0.5");
                            }

                            user.IsSit = true;
                            user.UpdateNeeded = true;
                        }
                        break;
                    }

                case "lay":
                    {
                        if (user.RotBody % 2 == 0)
                        {
                            if (user.IsTransf)
                            {
                                user.SetStatus("lay", "");
                            }
                            else
                            {
                                user.SetStatus("lay", "0.7");
                            }

                            user.IsLay = true;
                            user.UpdateNeeded = true;
                        }
                        break;
                    }
                case "transf":
                    {
                        int raceId = 0;
                        string petName = Value;
                        if (Value.Contains(" "))
                        {
                            if (int.TryParse(Value.Split(' ')[1], out raceId))
                            {
                                if (raceId < 1 || raceId > 50)
                                {
                                    raceId = 0;
                                }
                            }

                            petName = Value.Split(' ')[0];
                        }

                        if (user.SetPetTransformation(petName, raceId))
                        {
                            user.IsTransf = true;

                            user.Room.SendPacket(new UserRemoveComposer(user.VirtualId));
                            user.Room.SendPacket(new UsersComposer(user));
                        }
                        break;
                    }
                case "transfstop":
                    {
                        user.IsTransf = false;

                        user.Room.SendPacket(new UserRemoveComposer(user.VirtualId));
                        user.Room.SendPacket(new UsersComposer(user));
                        break;
                    }
                case "coins":
                    {
                        if (!int.TryParse(Value, out int ValueNumber))
                        {
                            return;
                        }

                        user.GetClient().GetUser().Credits += ValueNumber;
                        user.GetClient().SendPacket(new CreditBalanceComposer(user.GetClient().GetUser().Credits));
                        break;
                    }
                case "badge":
                    {
                        user.GetClient().GetUser().GetBadgeComponent().GiveBadge(Value, true);
                        user.GetClient().SendPacket(new ReceiveBadgeComposer(Value));
                        break;
                    }
                case "removebadge":
                    {
                        user.GetClient().GetUser().GetBadgeComponent().RemoveBadge(Value);
                        user.GetClient().SendPacket(new BadgesComposer(user.GetClient().GetUser().GetBadgeComponent().BadgeList));
                        break;
                    }

                case "send":
                    {
                        if (int.TryParse(Value, out int RoomId))
                        {
                            user.GetClient().GetUser().IsTeleporting = true;
                            user.GetClient().GetUser().TeleportingRoomID = RoomId;
                            user.GetClient().SendPacket(new RoomForwardComposer(RoomId));
                        }
                        break;
                    }
                case "alert":
                    {
                        user.GetClient().SendNotification(Value);
                        break;
                    }
                case "achievement":
                    {
                        WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), Value, 1);
                        break;
                    }
                case "winmovierun":
                    {
                        if (user.IsBot || user.GetClient() == null || user.GetClient().GetUser() == null || user.GetClient().GetUser().Rank > 4)
                        {
                            break;
                        }


                        if (user.GetUsername() == user.Room.RoomData.OwnerName)
                        {
                            break;
                        }

                        using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            UserDao.UpdateAddRunPoints(dbClient, user.GetClient().GetUser().Id);
                        }

                        break;
                    }
                case "givelot":
                    {
                        if (user.IsBot || user.GetClient() == null || user.GetClient().GetUser() == null || user.GetClient().GetUser().Rank > 4)
                        {
                            break;
                        }

                        if(user.GetUsername() == user.Room.RoomData.OwnerName)
                        {
                            break;
                        }

                        List<string> allowedOwner = new List<string>{ "LieuPublic", "MovieRunOff", "WibboGame", "WorldRunOff", "officialrooms" };

                        if(!allowedOwner.Contains(user.Room.RoomData.OwnerName))
                        {
                            break;
                        }

                        if (user.WiredGivelot)
                        {
                            break;
                        }

                        user.WiredGivelot = true;

                        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(12018410, out ItemData ItemData))
                        {
                            break;
                        }

                        int NbLot = WibboEnvironment.GetRandomNumber(1, 3);

                        if (user.GetClient().GetUser().Rank > 1)
                        {
                            NbLot = WibboEnvironment.GetRandomNumber(3, 5);
                        }

                        int NbLotDeluxe = WibboEnvironment.GetRandomNumber(1, 4);
                        if (user.GetClient().GetUser().Rank > 1)
                        {
                            NbLotDeluxe = WibboEnvironment.GetRandomNumber(3, 4);
                        }

                        int NbBadge = WibboEnvironment.GetRandomNumber(1, 2);
                        if (user.GetClient().GetUser().Rank > 1)
                        {
                            NbBadge = WibboEnvironment.GetRandomNumber(2, 3);
                        }

                        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(91947063, out ItemData ItemDataBadge))
                        {
                            return;
                        }

                        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(618784, out ItemData ItemDataDeluxe))
                        {
                            return;
                        }

                        List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, user.GetClient().GetUser(), "", NbLot);
                        Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataBadge, user.GetClient().GetUser(), "", NbBadge));
                        if (NbLotDeluxe == 4)
                        {
                            Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataDeluxe, user.GetClient().GetUser(), "", 1));
                        }

                        foreach (Item PurchasedItem in Items)
                        {
                            if (user.GetClient().GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
                            {
                                user.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                            }
                        }

                        string DeluxeMessage = (NbLotDeluxe == 4) ? " Et une RareBox Deluxe !" : "";
                        user.GetClient().SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", user.GetClient().Langue), NbLot, NbBadge) + DeluxeMessage);

                        using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            UserDao.UpdateAddGamePoints(dbClient, user.GetClient().GetUser().Id);
                        }

                        WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_Extrabox", 1);
                        WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(1953042, user.Room.RoomData.OwnerName, user.RoomId, string.Empty, "givelot", "SuperWired givelot: " + user.GetUsername());

                        break;
                    }
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            int delay;
            if (int.TryParse(row["delay"].ToString(), out delay))
	            this.Delay = delay;

            if (int.TryParse(row["trigger_data_2"].ToString(), out delay))
                this.Delay = delay;

            this.StringParam = row["trigger_data"].ToString();

        }
    }
}
