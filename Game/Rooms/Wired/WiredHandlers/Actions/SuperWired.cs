using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Roleplay;
using Butterfly.Game.Roleplay.Enemy;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
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
                effet = this.StringParam.Split(':')[0].ToLower();
            }
            else
            {
                effet = this.StringParam;
            }

            switch (effet)
            {
                case "rpsendtimeuser":
                case "TimeSpeed":
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
                case "removemoney1":
                case "removemoney2":
                case "removemoney3":
                case "removemoney4":
                case "addmoney":
                case "addmoney1":
                case "addmoney2":
                case "addmoney3":
                case "addmoney4":
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

            string command = "";
            string value = "";

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

                        RPEnemy RPEnemyConfig = null;

                        if (!BotOrPet.IsPet)
                        {
                            RPEnemyConfig = ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyBot(BotOrPet.BotData.Id);
                        }
                        else
                        {
                            RPEnemyConfig = ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyPet(BotOrPet.BotData.Id);
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateZoneDistance(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "resetposition":
                                {
                                    RPEnemyConfig.ResetPosition = (Params[2] == "true");
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        RoleplayEnemyDao.UpdateLostAggroDistance(dbClient, RPEnemyConfig.Id, ParamInt);
                                    }

                                    break;
                                }
                            case "zombiemode":
                                {
                                    RPEnemyConfig.ZombieMode = (Params[2] == "true");
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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
                            ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().RemoveEnemyBot(BotOrPet.BotData.Id);
                            BotOrPet.BotData.RoleBot = null;
                            BotOrPet.BotData.AiType = AI.AIType.Generic;
                            BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                        }
                        else
                        {
                            ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().RemoveEnemyPet(BotOrPet.BotData.Id);
                            BotOrPet.BotData.RoleBot = null;
                            BotOrPet.BotData.AiType = AI.AIType.Pet;
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
                            RPEnemy RPEnemyConfig = ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().AddEnemyBot(BotOrPet.BotData.Id);
                            if (RPEnemyConfig != null)
                            {
                                BotOrPet.BotData.RoleBot = new RoleBot(RPEnemyConfig);
                                BotOrPet.BotData.AiType = AI.AIType.RolePlayBot;
                                BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                            }
                        }
                        else
                        {
                            RPEnemy RPEnemyConfig = ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().AddEnemyPet(BotOrPet.BotData.Id);
                            if (RPEnemyConfig != null)
                            {
                                BotOrPet.BotData.RoleBot = new RoleBot(RPEnemyConfig);
                                BotOrPet.BotData.AiType = AI.AIType.RolePlayPet;
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
                            Room room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                            if (room != null && room.RoomData.OwnerId == room.RoomData.OwnerId)
                            {
                                user.GetClient().GetHabbo().IsTeleporting = true;
                                user.GetClient().GetHabbo().TeleportingRoomID = RoomId;
                                user.GetClient().GetHabbo().PrepareRoom(RoomId, "");
                            }
                        }
                        break;
                    }
                case "inventoryadd":
                    {
                        int.TryParse(value, out int ItemId);

                        RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
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

                        RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
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

                                RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Id);
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

                            RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Id);
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
                case "removehygiene":
                    {
                        int.TryParse(value, out int Nb);

                        Rp.RemoveHygiene(Nb);

                        Rp.SendUpdate();
                        break;
                    }
                case "addhygiene":
                    {
                        int.TryParse(value, out int Nb);

                        Rp.AddHygiene(Nb);

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

                        Rp.WeaponGun = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(Nb);

                        break;
                    }
                case "weaponcacid":
                    {
                        int.TryParse(value, out int Nb);

                        if (Nb < 0 || Nb > 3)
                        {
                            Nb = 0;
                        }

                        Rp.WeaponCac = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(Nb);
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
                case "removemoney1":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        if (Rp.Money1 - Nb < 0)
                        {
                            Rp.Money1 = 0;
                        }
                        else
                        {
                            Rp.Money1 -= Nb;
                        }
                        Rp.SendUpdate();
                        break;
                    }
                case "addmoney1":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.Money1 += Nb;
                        Rp.SendUpdate();
                        break;
                    }
                case "removemoney2":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        if (Rp.Money2 - Nb < 0)
                        {
                            Rp.Money2 = 0;
                        }
                        else
                        {
                            Rp.Money2 -= Nb;
                        }
                        Rp.SendUpdate();
                        break;
                    }
                case "addmoney2":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.Money2 += Nb;
                        Rp.SendUpdate();
                        break;
                    }
                case "removemoney3":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        if (Rp.Money3 - Nb < 0)
                        {
                            Rp.Money3 = 0;
                        }
                        else
                        {
                            Rp.Money3 -= Nb;
                        }
                        Rp.SendUpdate();
                        break;
                    }
                case "addmoney3":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.Money3 += Nb;
                        Rp.SendUpdate();
                        break;
                    }
                case "removemoney4":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        if (Rp.Money4 - Nb < 0)
                        {
                            Rp.Money4 = 0;
                        }
                        else
                        {
                            Rp.Money4 -= Nb;
                        }
                        Rp.SendUpdate();
                        break;
                    }
                case "addmoney4":
                    {
                        int.TryParse(value, out int Nb);
                        if (Nb <= 0)
                        {
                            break;
                        }

                        Rp.Money4 += Nb;
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
                        if (int.TryParse(value, out int danceid))
                        {
                            if (danceid < 0 || danceid > 4)
                            {
                                danceid = 0;
                            }

                            if (danceid > 0 && user.CarryItemID > 0)
                            {
                                user.CarryItem(0);
                            }

                            user.DanceId = danceid;
                            user.Room.SendPacket(new DanceComposer(user, danceid));
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

                        if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, false))
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
                        Seconde = Seconde * 2;
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
                            if (RUser != null && !RUser.IsBot && !RUser.GetClient().GetHabbo().HasFuse("fuse_no_kick") && this.RoomInstance.RoomData.OwnerId != RUser.UserId)
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
                            if (RUser != null && !RUser.IsBot && !RUser.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                            {
                                RUser.GetClient().SendNotification(value);
                            }
                        }
                        break;
                    }
                case "stopsoundroom":
                    {
                        this.RoomInstance.SendPacketWeb(new StopSoundComposer(value));
                        break;
                    }
                case "playsoundroom":
                    {
                        this.RoomInstance.SendPacketWeb(new PlaySoundComposer(value, 1)); //Type = Trax
                        break;
                    }
                case "playmusicroom":
                    {
                        this.RoomInstance.SendPacketWeb(new PlaySoundComposer(value, 2, true)); //Type = Trax
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

                                    if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(IntValue, false))
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
                        RoomUser Bot = null;
                        if (ButterflyEnvironment.GetRandomNumber(0, 1) == 1)
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

                        string TextMessage = Phrases[ButterflyEnvironment.GetRandomNumber(0, Phrases.Count - 1)];
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
                        this.RoomInstance.GetGameMap().updateMapForItem(item);

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
                        this.RoomInstance.GetGameMap().updateMapForItem(item);

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

        private void UserCommand(string Cmd, string Value, RoomUser User, Item TriggerItem)
        {
            if (User == null || User.IsBot || User.GetClient() == null)
            {
                return;
            }

            switch (Cmd)
            {
                case "usermute":
                    {
                        User.muted = (Value == "true") ? true : false;
                        break;
                    }
                case "botchoosenav":
                    {
                        List<string[]> ChooseList = new List<string[]>();

                        if (string.IsNullOrEmpty(Value))
                        {
                            User.GetClient().GetHabbo().SendWebPacket(new BotChooseComposer(ChooseList));
                            break;
                        }

                        foreach (string RoomIdString in Value.Split(','))
                        {
                            if (!int.TryParse(RoomIdString, out int RoomId))
                            {
                                continue;
                            }

                            RoomData RoomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

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

                        User.GetClient().GetHabbo().SendWebPacket(new BotChooseComposer(ChooseList));

                        break;
                    }
                case "botchoose":
                    {
                        List<string[]> ChooseList = new List<string[]>();
                        if (string.IsNullOrEmpty(Value))
                        {
                            User.GetClient().GetHabbo().SendWebPacket(new BotChooseComposer(ChooseList));
                            break;
                        }

                        if (Value.Contains(","))
                        {
                            foreach (string pChoose in Value.Split(','))
                            {
                                List<string> list = pChoose.Split(';').ToList();
                                if (list.Count == 3)
                                {
                                    RoomUser BotOrPet = User.Room.GetRoomUserManager().GetBotByName(list[0]);
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
                                RoomUser BotOrPet = User.Room.GetRoomUserManager().GetBotByName(list[0]);
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

                        User.GetClient().GetHabbo().SendWebPacket(new BotChooseComposer(ChooseList));

                        break;
                    }
                case "forcesound":
                    {
                        User.GetClient().GetHabbo().ClientVolume.Clear();
                        User.GetClient().GetHabbo().ClientVolume.Add(100);
                        User.GetClient().GetHabbo().ClientVolume.Add(100);
                        User.GetClient().GetHabbo().ClientVolume.Add(100);

                        User.GetClient().GetHabbo().SendWebPacket(new SettingVolumeComposer(100, 100, 100));
                        break;
                    }
                case "stopsounduser":
                    {
                        User.GetClient().GetHabbo().SendWebPacket(new StopSoundComposer(Value)); //Type = Trax

                        break;
                    }
                case "playsounduser":
                    {
                        User.GetClient().GetHabbo().SendWebPacket(new PlaySoundComposer(Value, 1)); //Type = furni

                        break;
                    }
                case "playmusicuser":
                    {
                        User.GetClient().GetHabbo().SendWebPacket(new PlaySoundComposer(Value, 2, true)); //Type = Trax

                        break;
                    }
                case "moveto":
                    {
                        if (Value == "true")
                        {
                            User.AllowMoveTo = true;
                        }
                        else
                        {
                            User.AllowMoveTo = false;
                        }

                        break;
                    }
                case "reversewalk":
                    {
                        if (Value == "true")
                        {
                            User.ReverseWalk = true;
                        }
                        else
                        {
                            User.ReverseWalk = false;
                        }

                        break;
                    }
                case "speedwalk":
                    {
                        if (Value == "true")
                        {
                            User.WalkSpeed = true;
                        }
                        else
                        {
                            User.WalkSpeed = false;
                        }

                        break;
                    }
                case "openpage":
                    {
                        User.GetClient().SendPacket(new NuxAlertComposer("habbopages/" + Value));
                        break;
                    }
                case "rot":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (ValueInt > 7 || ValueInt < 0)
                        {
                            ValueInt = 0;
                        }

                        if (User.RotBody == ValueInt && User.RotHead == ValueInt)
                        {
                            break;
                        }

                        User.RotBody = ValueInt;
                        User.RotHead = ValueInt;
                        User.UpdateNeeded = true;

                        break;
                    }
                case "stand":
                    {
                        if (User.Statusses.ContainsKey("lay"))
                        {
                            User.RemoveStatus("lay");
                        }

                        if (User.Statusses.ContainsKey("sit"))
                        {
                            User.RemoveStatus("sit");
                        }

                        if (User.Statusses.ContainsKey("sign"))
                        {
                            User.RemoveStatus("sign");
                        }

                        User.UpdateNeeded = true;
                        break;
                    }
                case "allowshoot":
                    {
                        if (Value == "true")
                        {
                            User.AllowShoot = true;
                        }
                        else
                        {
                            User.AllowShoot = false;
                        }

                        break;
                    }
                case "addpointteam":
                    {
                        if (User.Team == Games.Team.none)
                        {
                            break;
                        }

                        int.TryParse(Value, out int Count);

                        if (User.Room == null)
                        {
                            break;
                        }

                        User.Room.GetGameManager().AddPointToTeam(User.Team, Count, User);
                        break;
                    }
                case "ingame":
                    {
                        if (Value == "true")
                        {
                            User.InGame = true;
                        }
                        else
                        {
                            User.InGame = false;
                        }

                        break;
                    }
                case "usertimer":
                    {
                        int.TryParse(Value, out int Points);

                        User.UserTimer = Points;

                        break;
                    }
                case "addusertimer":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        User.UserTimer += Points;

                        break;
                    }
                case "removeusertimer":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        if (Points >= User.UserTimer)
                        {
                            User.UserTimer = 0;
                        }
                        else
                        {
                            User.UserTimer -= Points;
                        }

                        break;
                    }
                case "point":
                    {
                        int.TryParse(Value, out int Points);

                        User.WiredPoints = Points;

                        break;
                    }
                case "addpoint":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        User.WiredPoints += Points;

                        break;
                    }
                case "removepoint":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                        {
                            break;
                        }

                        if (Points >= User.WiredPoints)
                        {
                            User.WiredPoints = 0;
                        }
                        else
                        {
                            User.WiredPoints -= Points;
                        }
                        break;
                    }
                case "freeze":
                    {
                        int.TryParse(Value, out int Seconde);
                        Seconde = Seconde * 2;
                        User.Freeze = true;
                        User.FreezeEndCounter = Seconde;
                        break;
                    }
                case "unfreeze":
                    {
                        User.Freeze = false;
                        User.FreezeEndCounter = 0;
                        break;
                    }
                case "breakwalk":
                    {
                        if (Value == "true")
                        {
                            User.BreakWalkEnable = true;
                        }
                        else
                        {
                            User.BreakWalkEnable = false;
                        }

                        break;
                    }
                case "enable":
                    {
                        if (!int.TryParse(Value, out int NumEnable))
                        {
                            return;
                        }

                        if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, false))
                        {
                            return;
                        }

                        User.ApplyEffect(NumEnable);
                        break;
                    }
                case "enablestaff":
                    {
                        if (!int.TryParse(Value, out int NumEnable))
                        {
                            return;
                        }

                        if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, true))
                        {
                            return;
                        }

                        User.ApplyEffect(NumEnable);
                        break;
                    }
                case "dance":
                    {
                        if (User.Room == null)
                        {
                            break;
                        }

                        if (int.TryParse(Value, out int danceid))
                        {
                            if (danceid < 0 || danceid > 4)
                            {
                                danceid = 0;
                            }

                            if (danceid > 0 && User.CarryItemID > 0)
                            {
                                User.CarryItem(0);
                            }

                            User.DanceId = danceid;
                            ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_DANCE);
                            Message.WriteInteger(User.VirtualId);
                            Message.WriteInteger(danceid);
                            User.Room.SendPacket(Message);
                        }
                        break;
                    }
                case "handitem":
                    {
                        if (int.TryParse(Value, out int carryid))
                        {
                            User.CarryItem(carryid, true);
                        }

                        break;
                    }
                case "sit":
                    {
                        if (User.RotBody % 2 == 0)
                        {
                            if (User.transformation)
                            {
                                User.SetStatus("sit", "");
                            }
                            else
                            {
                                User.SetStatus("sit", "0.5");
                            }

                            User.IsSit = true;
                            User.UpdateNeeded = true;
                        }
                        break;
                    }

                case "lay":
                    {
                        if (User.RotBody % 2 == 0)
                        {
                            if (User.transformation)
                            {
                                User.SetStatus("lay", "");
                            }
                            else
                            {
                                User.SetStatus("lay", "0.7");
                            }

                            User.IsLay = true;
                            User.UpdateNeeded = true;
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

                        if (User.SetPetTransformation(petName, raceId))
                        {
                            User.transformation = true;

                            User.Room.SendPacket(new UserRemoveComposer(User.VirtualId));
                            User.Room.SendPacket(new UsersComposer(User));
                        }
                        break;
                    }
                case "transfstop":
                    {
                        User.transformation = false;

                        User.Room.SendPacket(new UserRemoveComposer(User.VirtualId));
                        User.Room.SendPacket(new UsersComposer(User));
                        break;
                    }
                case "coins":
                    {
                        if (!int.TryParse(Value, out int ValueNumber))
                        {
                            return;
                        }

                        User.GetClient().GetHabbo().Credits += ValueNumber;
                        User.GetClient().SendPacket(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                        break;
                    }
                case "badge":
                    {
                        User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Value, true);
                        User.GetClient().SendPacket(new ReceiveBadgeComposer(Value));
                        break;
                    }
                case "removebadge":
                    {
                        User.GetClient().GetHabbo().GetBadgeComponent().RemoveBadge(Value);
                        User.GetClient().SendPacket(User.GetClient().GetHabbo().GetBadgeComponent().Serialize());
                        break;
                    }

                case "send":
                    {
                        if (int.TryParse(Value, out int RoomId))
                        {
                            User.GetClient().GetHabbo().IsTeleporting = true;
                            User.GetClient().GetHabbo().TeleportingRoomID = RoomId;
                            User.GetClient().GetHabbo().PrepareRoom(RoomId, "");
                        }
                        break;
                    }
                case "alert":
                    {
                        User.GetClient().SendNotification(Value);
                        break;
                    }
                case "achievement":
                    {
                        ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), Value, 1);
                        break;
                    }
                case "winmovierun":
                    {
                        if (User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().Rank > 4)
                        {
                            break;
                        }

                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            UserDao.UpdateAddRunPoints(dbClient, User.GetClient().GetHabbo().Id);
                        }

                        break;
                    }
                case "givelot":
                    {
                        if (User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().Rank > 4)
                        {
                            break;
                        }

                        if (User.WiredGivelot)
                        {
                            break;
                        }

                        User.WiredGivelot = true;

                        if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(12018410, out ItemData ItemData))
                        {
                            break;
                        }

                        int NbLot = ButterflyEnvironment.GetRandomNumber(1, 3);

                        if (User.GetClient().GetHabbo().Rank > 1)
                        {
                            NbLot = ButterflyEnvironment.GetRandomNumber(3, 5);
                        }

                        int NbLotDeluxe = ButterflyEnvironment.GetRandomNumber(1, 4);
                        if (User.GetClient().GetHabbo().Rank > 1)
                        {
                            NbLotDeluxe = ButterflyEnvironment.GetRandomNumber(3, 4);
                        }

                        int NbBadge = ButterflyEnvironment.GetRandomNumber(1, 2);
                        if (User.GetClient().GetHabbo().Rank > 1)
                        {
                            NbBadge = ButterflyEnvironment.GetRandomNumber(2, 3);
                        }

                        if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(91947063, out ItemData ItemDataBadge))
                        {
                            return;
                        }

                        if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(618784, out ItemData ItemDataDeluxe))
                        {
                            return;
                        }

                        List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, User.GetClient().GetHabbo(), "", NbLot);
                        Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataBadge, User.GetClient().GetHabbo(), "", NbBadge));
                        if (NbLotDeluxe == 4)
                        {
                            Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataDeluxe, User.GetClient().GetHabbo(), "", 1));
                        }

                        foreach (Item PurchasedItem in Items)
                        {
                            if (User.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                            {
                                User.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                            }
                        }

                        string DeluxeMessage = (NbLotDeluxe == 4) ? " Et une RareBox Deluxe !" : "";
                        User.GetClient().SendNotification(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", User.GetClient().Langue), NbLot, NbBadge) + DeluxeMessage);

                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            UserDao.UpdateAddGamePoints(dbClient, User.GetClient().GetHabbo().Id);
                        }

                        ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_Extrabox", 1);
                        ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(1953042, User.Room.RoomData.OwnerName, User.RoomId, string.Empty, "givelot", "SuperWired givelot: " + User.GetUsername());

                        break;
                    }
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, this.DelayCycle.ToString(), this.StringParam, false, null, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            int delay = 0;
            if (int.TryParse(row["delay"].ToString(), out delay))
	            this.Delay = delay;

            if (int.TryParse(row["trigger_data_2"].ToString(), out delay))
                this.Delay = delay;

            this.StringParam = row["trigger_data"].ToString();

        }
    }
}
