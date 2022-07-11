using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.RolePlay;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Roleplay.Weapon;
using WibboEmulator.Game.Rooms;
using System.Collections.Concurrent;
using System.Data;

namespace WibboEmulator.Game.Roleplay.Player
{
    public class RolePlayer
    {
        private readonly int _rpId;
        private readonly int _id;

        private readonly ConcurrentDictionary<int, RolePlayInventoryItem> _inventory;

        public int Health;
        public int HealthMax;

        public int Money;
        public int Munition;
        public int GunLoad;
        public int Exp;
        public bool Dead;
        public bool SendPrison;
        public int Level;
        public RPWeapon WeaponGun;
        public RPWeapon WeaponCac;
        public int Energy;
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

        public RolePlayer(int pRpId, int pId, int pHealth, int pMoney, int pMunition, int pExp, int pEnergy, int pWeaponGun, int pWeaponCac)
        {
            this._rpId = pRpId;
            this._id = pId;
            this.Health = pHealth;
            this.Energy = pEnergy;
            this.Money = pMoney;
            this.Munition = pMunition;
            this.Exp = pExp;
            this.PvpEnable = true;
            this.WeaponCac = WibboEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(pWeaponCac);
            this.WeaponGun = WibboEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(pWeaponGun);

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
            this.Money = 0;
            this.Munition = 0;
            this.Exp = 0;
            this.Level = 1;
            this.HealthMax = 100;

            this.WeaponCac = WibboEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(0);
            this.WeaponGun = WibboEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(0);

            this._inventory.Clear();

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserRoleplayItemDao.Delete(dbClient, this._id, this._rpId);
                UserRoleplayDao.Delete(dbClient, this._id, this._rpId);
                UserRoleplayDao.Update(dbClient, this._id, this._rpId, this.Health, this.Energy, this.Money, this.Munition, this.Exp, this.WeaponGun.Id, this.WeaponCac.Id);
            }

            this.SendPacket(new LoadInventoryRpComposer(this._inventory));
            this.SendUpdate();
        }

        public void LoadInventory()
        {
            DataTable Table;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                Table = UserRoleplayItemDao.GetAll(dbClient, this._id, this._rpId);

            foreach (DataRow dataRow in Table.Rows)
            {
                if (!this._inventory.ContainsKey(Convert.ToInt32(dataRow["item_id"])))
                {
                    this._inventory.TryAdd(Convert.ToInt32(dataRow["item_id"]), new RolePlayInventoryItem(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["item_id"]), Convert.ToInt32(dataRow["count"])));
                }
            }


            this.SendPacket(new LoadInventoryRpComposer(this._inventory));
        }

        internal RolePlayInventoryItem GetInventoryItem(int Id)
        {
            this._inventory.TryGetValue(Id, out RolePlayInventoryItem Item);

            return Item;
        }

        internal void AddInventoryItem(int itemId, int count = 1)
        {
            RPItem RPItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(itemId);
            if (RPItem == null)
            {
                return;
            }

            RolePlayInventoryItem Item = this.GetInventoryItem(itemId);
            if (Item == null)
            {
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                int Id = UserRoleplayItemDao.Insert(dbClient, this._id, this._rpId, itemId, count);
                this._inventory.TryAdd(itemId, new RolePlayInventoryItem(Id, itemId, count));
            }
            else
            {
                Item.Count += count;
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserRoleplayItemDao.UpdateAddCount(dbClient, Item.Id, count);
            }


            this.SendPacket(new AddInventoryItemRpComposer(RPItem, count));
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
                Item.Count -= Count;

                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserRoleplayItemDao.UpdateRemoveCount(dbClient, Item.Id, Count);
            }
            else
            {
                this._inventory.TryRemove(ItemId, out Item);

                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserRoleplayItemDao.Delete(dbClient, this._id);
            }

            this.SendPacket(new RemoveItemInventoryRpComposer(ItemId, Count));
        }

        public void SendPacket(IServerPacket Message)
        {
            Client session = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(this._id);
            if (session != null)
            {
                session.SendPacket(Message);
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
                    User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.userdead", User.GetClient().Langue));
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
                            User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.hitslow", User.GetClient().Langue));
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
                        User.OnChatMe(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), 0, true);
                    }
                    else
                    {
                        User.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), 0, true);
                    }
                }
            }

            this.SendUpdate();
        }

        public void SendUpdate(bool SendNow = false)
        {
            if (SendNow)
            {
                this.SendPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Money, this.Munition, this.Level));
            }
            else
            {
                this.NeedUpdate = true;
            }
        }

        public void SendItemsList(List<RPItem> ItemsList)
        {
            this.SendPacket(new BuyItemsListComposer(ItemsList));
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
                    User.GetClient().GetUser().IsTeleporting = true;
                    User.GetClient().GetUser().TeleportingRoomID = RPManager.PrisonId;
                    User.GetClient().SendPacket(new RoomForwardComposer(RPManager.PrisonId));
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
                    User.GetClient().GetUser().IsTeleporting = true;
                    User.GetClient().GetUser().TeleportingRoomID = RPManager.HopitalId;
                    User.GetClient().SendPacket(new RoomForwardComposer(RPManager.HopitalId));
                }
            }

            if (this.NeedUpdate)
            {
                this.NeedUpdate = false;
                this.SendPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Money, this.Munition, this.Level));
            }
        }

        public void Destroy()
        {
            if (this.Dispose)
            {
                return;
            }

            this.Dispose = true;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserRoleplayDao.Update(dbClient, this._id, this._rpId, this.Health, this.Energy, this.Money, this.Munition, this.Exp, this.WeaponGun.Id, this.WeaponCac.Id);
            }

            this.SendPacket(new RpStatsComposer(0, 0, 0, 0, 0, 0, 0));
            this._inventory.Clear();
        }
    }
}