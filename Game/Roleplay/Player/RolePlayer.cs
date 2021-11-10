using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Roleplay.Weapon;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Roleplay.Player
{
    public class RolePlayer
    {
        private readonly int _rpId;
        private readonly int _id;

        private readonly ConcurrentDictionary<int, RolePlayInventoryItem> _inventory;

        public int Health;
        public int HealthMax;

        public int Money;
        public int Money1;
        public int Money2;
        public int Money3;
        public int Money4;
        public int Munition;
        public int GunLoad;
        public int Exp;
        public bool Dead;
        public bool SendPrison;
        public int Level;
        public RPWeapon WeaponGun;
        public RPWeapon WeaponCac;
        public int Energy;
        public int Hygiene;
        public int GunLoadTimer;
        public int PrisonTimer;
        public int DeadTimer;
        public int SlowTimer;
        public int AggroTimer;
        public int PlayerOutTimer;
        public bool PvpEnable;

        public int TradeId;
        public bool NeedUpdate;
        public bool Dispose;

        public RolePlayer(int pRpId, int pId, int pHealth, int pMoney, int pMoney1, int pMoney2, int pMoney3, int pMoney4, int pMunition, int pExp, int pEnergy, int pHygiene, int pWeaponGun, int pWeaponCac)
        {
            this._rpId = pRpId;
            this._id = pId;
            this.Health = pHealth;
            this.Energy = pEnergy;
            this.Hygiene = pHygiene;
            this.Money = pMoney;
            this.Money1 = pMoney1;
            this.Money2 = pMoney2;
            this.Money3 = pMoney3;
            this.Money4 = pMoney4;
            this.Munition = pMunition;
            this.Exp = pExp;
            this.PvpEnable = true;
            this.WeaponCac = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(pWeaponCac);
            this.WeaponGun = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(pWeaponGun);

            this.GunLoad = 6;
            this.GunLoadTimer = 0;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {

                int expmax = (i * 50) + (i * 10) * i;

                if (this.Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }
            this.Level = Level;
            this.HealthMax = 90 + (this.Level * 10);

            this.SendPrison = false;
            this.PrisonTimer = 0;
            this.Dead = false;
            this.DeadTimer = 0;

            this.AggroTimer = 0;
            this.SlowTimer = 0;

            this._inventory = new ConcurrentDictionary<int, RolePlayInventoryItem>();

            this.TradeId = 0;
        }

        public void Reset()
        {
            this.Health = 100;
            this.Energy = 100;
            this.Hygiene = 100;
            this.Money = 0;
            this.Money1 = 0;
            this.Money2 = 0;
            this.Money3 = 0;
            this.Money4 = 0;
            this.Munition = 0;
            this.Exp = 0;
            this.Level = 1;
            this.HealthMax = 100;

            this.WeaponCac = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(0);
            this.WeaponGun = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(0);

            this._inventory.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserRoleplayItemDao.Delete(dbClient, this._id, this._rpId);
                UserRoleplayDao.Delete(dbClient, this._id, this._rpId);
                UserRoleplayDao.Update(dbClient, this._id, this._rpId, this.Health, this.Energy, this.Hygiene, this.Money, this.Money1, this.Money2, this.Money3, this.Money4, this.Munition, this.Exp, this.WeaponGun.Id, this.WeaponCac.Id);
            }

            this.SendWebPacket(new LoadInventoryRpComposer(this._inventory));
            this.SendUpdate();
        }

        public void LoadInventory()
        {
            DataTable Table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                Table = UserRoleplayItemDao.GetAll(dbClient, this._id, this._rpId);

            foreach (DataRow dataRow in Table.Rows)
            {
                if (!this._inventory.ContainsKey(Convert.ToInt32(dataRow["item_id"])))
                {
                    this._inventory.TryAdd(Convert.ToInt32(dataRow["item_id"]), new RolePlayInventoryItem(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["item_id"]), Convert.ToInt32(dataRow["count"])));
                }
            }


            this.SendWebPacket(new LoadInventoryRpComposer(this._inventory));
        }

        internal RolePlayInventoryItem GetInventoryItem(int Id)
        {
            this._inventory.TryGetValue(Id, out RolePlayInventoryItem Item);

            return Item;
        }

        internal void AddInventoryItem(int itemId, int count = 1)
        {
            RPItem RPItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(itemId);
            if (RPItem == null)
            {
                return;
            }

            RolePlayInventoryItem Item = this.GetInventoryItem(itemId);
            if (Item == null)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    int Id = UserRoleplayItemDao.Insert(dbClient, this._id, this._rpId, itemId, count);
                    this._inventory.TryAdd(itemId, new RolePlayInventoryItem(Id, itemId, count));
                }
            }
            else
            {
                Item.Count += count;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserRoleplayItemDao.UpdateAddCount(dbClient, Item.Id, count);
                }
            }


            this.SendWebPacket(new AddInventoryItemRpComposer(RPItem, count));
        }

        internal void RemoveInventoryItem(int ItemId, int Count = 1)
        {
            RolePlayInventoryItem Item = this.GetInventoryItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (Item.Count > Count)
            {
                Item.Count = Item.Count - Count;

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserRoleplayItemDao.UpdateRemoveCount(dbClient, Item.Id, Count);
                }
            }
            else
            {
                this._inventory.TryRemove(ItemId, out Item);

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserRoleplayItemDao.Delete(dbClient, this._id);
                }
            }

            this.SendWebPacket(new RemoveItemInventoryRpComposer(ItemId, Count));
        }

        public void SendWebPacket(IServerPacket Message)
        {
            WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(this._id);
            if (ClientWeb != null)
            {
                ClientWeb.SendPacket(Message);
            }
        }

        internal void RemoveMunition(int Nb)
        {
            if (this.Munition - Nb <= 0)
            {
                this.Munition = 0;
            }
            else
            {
                this.Munition -= Nb;
            }
        }

        internal void AddMunition(int Nb)
        {
            if (Nb <= 0)
            {
                return;
            }

            if (Nb > 99)
            {
                Nb = 99;
            }

            if (this.Munition + Nb > 99)
            {
                this.Munition = 99;
            }
            else
            {
                this.Munition += Nb;
            }
        }

        internal void AddHealth(int Nb)
        {
            if (Nb <= 0)
            {
                return;
            }

            if (this.Health + Nb > this.HealthMax)
            {
                this.Health = this.HealthMax;
            }
            else
            {
                this.Health += Nb;
            }
        }

        public void AddExp(int pNb)
        {
            this.Exp += pNb;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {
                int expmax = (i * 50) + (i * 10) * i;

                if (this.Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }

            if (this.Level < Level)
            {
                this.Level = Level;
                this.HealthMax = 90 + (this.Level * 10);
                this.Health = this.HealthMax;
                this.SendUpdate();
            }
        }

        public void RemoveExp(int pNb)
        {
            if (this.Exp >= pNb)
            {
                this.Exp -= pNb;
            }
            else
            {
                this.Exp = 0;
            }

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {
                int expmax = (i * 50) + (i * 10) * i;

                if (this.Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }

            if (this.Level != Level)
            {
                this.Level = Level;
                this.HealthMax = 90 + (this.Level * 10);
                this.Health = this.HealthMax;
                this.SendUpdate();
            }
        }

        internal void AddEnergy(int Nb)
        {
            if (this.Energy + Nb > 100)
            {
                this.Energy = 100;
            }
            else
            {
                this.Energy += Nb;
            }
        }

        internal void RemoveEnergy(int Nb)
        {
            if (this.Energy - Nb < 0)
            {
                this.Energy = 0;
            }
            else
            {
                this.Energy -= Nb;
            }
        }

        internal void AddHygiene(int Nb)
        {
            if (this.Hygiene + Nb > 100)
            {
                this.Hygiene = 100;
            }
            else
            {
                this.Hygiene += Nb;
            }
        }

        internal void RemoveHygiene(int Nb)
        {
            if (this.Hygiene - Nb < 0)
            {
                this.Hygiene = 0;
            }
            else
            {
                this.Hygiene -= Nb;
            }
        }

        public void Hit(RoomUser User, int Dmg, Room Room, bool Ralentie = false, bool Murmur = false, bool Aggro = true)
        {
            if (this.Dead || this.SendPrison)
            {
                return;
            }

            if (this.Health <= Dmg)
            {
                this.Health = 0;
                this.Dead = true;
                this.DeadTimer = 30;

                User.SetStatus("lay", "0.7");
                User.Freeze = true;
                User.FreezeEndCounter = 0;
                User.IsLay = true;
                User.UpdateNeeded = true;

                if (User.GetClient() != null)
                {
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.userdead", User.GetClient().Langue));
                }

                if (this.Money > 10)
                {
                    int monaiePerdu = (int)Math.Floor((double)(this.Money / 100) * 20);
                    this.Money -= monaiePerdu;

                    Room.GetRoomItemHandler().AddTempItem(User.VirtualId, 5461, User.SetX, User.SetY, User.Z, "1", monaiePerdu, InteractionTypeTemp.MONEY);
                }

                User.OnChat("A été mis K.O. ! [" + this.Health + "/" + this.HealthMax + "]", 0, true);
            }
            else
            {
                this.Health -= Dmg;
                if (Ralentie)
                {
                    if (this.SlowTimer == 0)
                    {
                        if (User.GetClient() != null)
                        {
                            User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.hitslow", User.GetClient().Langue));
                        }
                    }
                    this.SlowTimer = 6;
                }

                if (Aggro)
                {
                    this.AggroTimer = 30;
                }

                if (User.GetClient() != null)
                {
                    if (Murmur)
                    {
                        User.OnChatMe(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), 0, true);
                    }
                    else
                    {
                        User.OnChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), 0, true);
                    }
                }
            }

            this.SendUpdate();
        }

        public void SendUpdate(bool SendNow = false)
        {
            if (SendNow)
            {
                this.SendWebPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Hygiene, this.Money, this.Money1, this.Money2, this.Money3, this.Money4, this.Munition, this.Level));
            }
            else
            {
                this.NeedUpdate = true;
            }
        }

        public void SendItemsList(List<RPItem> ItemsList)
        {
            this.SendWebPacket(new BuyItemsListComposer(ItemsList));
        }

        public void OnCycle(RoomUser User, RolePlayerManager RPManager)
        {

            if (this.SlowTimer > 0)
            {
                this.SlowTimer--;

                User.BreakWalkEnable = true;
            }
            else
            {
                User.BreakWalkEnable = false;
            }

            if (this.PlayerOutTimer > 0)
            {
                this.PlayerOutTimer--;

                if (this.PlayerOutTimer == 0)
                {
                    this.AddEnergy(10);
                    this.NeedUpdate = true;

                    if (!this.Dead && !this.SendPrison)
                    {
                        User.Freeze = false;
                        User.IsSit = false;
                        User.RemoveStatus("sit");
                        User.UpdateNeeded = true;
                    }
                }
            }
            else
            {
                if (this.Energy <= 0)
                {
                    this.PlayerOutTimer = 60;

                    User.RotBody = 2;
                    User.RotHead = 2;
                    User.Freeze = true;
                    User.FreezeEndCounter = 0;
                    User.OnChat("*Tombe d'épuisement*");
                    User.SetStatus("sit", "0.5");
                    User.IsSit = true;
                    User.UpdateNeeded = true;

                    User.SendWhisperChat("Vous êtes tombé de fatiguer, reposez-vous pendants 30 secondes", true);
                }
            }


            if (this.GunLoadTimer > 0)
            {
                this.GunLoadTimer--;
                if (this.GunLoadTimer == 0)
                {
                    this.GunLoad = 6;
                }
            }
            else
            {
                if (this.GunLoad == 0)
                {
                    this.GunLoadTimer = 6;
                    User.OnChat("*Recharge mon arme*");
                }
            }


            if (this.AggroTimer > 0)
            {
                this.AggroTimer--;
            }

            if (this.SendPrison)
            {
                if (this.PrisonTimer > 0)
                {
                    this.PrisonTimer--;
                }
                else
                {
                    this.SendPrison = false;
                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = RPManager.PrisonId;
                    User.GetClient().GetHabbo().PrepareRoom(RPManager.PrisonId);
                }
            }

            if (this.Dead)
            {
                if (this.DeadTimer > 0)
                {
                    this.DeadTimer--;
                }
                else
                {
                    this.Dead = false;
                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = RPManager.HopitalId;
                    User.GetClient().GetHabbo().PrepareRoom(RPManager.HopitalId);
                }
            }

            if (this.NeedUpdate)
            {
                this.NeedUpdate = false;
                this.SendWebPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Hygiene, this.Money, this.Money1, this.Money2, this.Money3, this.Money4, this.Munition, this.Level));
            }
        }

        public void Destroy()
        {
            if (this.Dispose)
            {
                return;
            }

            this.Dispose = true;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserRoleplayDao.Update(dbClient, this._id, this._rpId, this.Health, this.Energy, this.Hygiene, this.Money, this.Money1, this.Money2, this.Money3, this.Money4, this.Munition, this.Exp, this.WeaponGun.Id, this.WeaponCac.Id);
            }

            this.SendWebPacket(new RpStatsComposer(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            this._inventory.Clear();
        }
    }
}