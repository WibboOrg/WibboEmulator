namespace WibboEmulator.Games.Roleplays.Player;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.RolePlay;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Roleplays.Item;
using WibboEmulator.Games.Roleplays.Weapon;
using WibboEmulator.Games.Rooms;

public class RolePlayer
{
    private readonly int _rpId;
    private readonly int _id;

    private readonly ConcurrentDictionary<int, RolePlayInventoryItem> _inventory;

    public int Health { get; set; }
    public int HealthMax { get; set; }

    public int Money { get; set; }
    public int Munition { get; set; }
    public int GunLoad { get; set; }
    public int Exp { get; set; }
    public bool Dead { get; set; }
    public bool SendPrison { get; set; }
    public int Level { get; set; }
    public RPWeapon WeaponGun { get; set; }
    public RPWeapon WeaponCac { get; set; }
    public int Energy { get; set; }
    public int GunLoadTimer { get; set; }
    public int PrisonTimer { get; set; }
    public int DeadTimer { get; set; }
    public int SlowTimer { get; set; }
    public int AggroTimer { get; set; }
    public int PlayerOutTimer { get; set; }
    public bool CacEnable { get; set; }
    public bool FarEnable { get; set; }
    public bool PvpEnable { get; set; }

    public int TradeId { get; set; }
    public bool NeedUpdate { get; set; }
    public bool Dispose { get; set; }

    public RolePlayer(int rpId, int id, int health, int money, int munition, int pxp, int energy, int weaponGun, int weaponCac)
    {
        this._rpId = rpId;
        this._id = id;
        this.Health = health;
        this.Energy = energy;
        this.Money = money;
        this.Munition = munition;
        this.Exp = pxp;
        this.PvpEnable = true;
        this.WeaponCac = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponCac(weaponCac);
        this.WeaponGun = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponGun(weaponGun);

        this.GunLoad = 6;
        this.GunLoadTimer = 0;

        var level = 1;
        for (var i = 1; i < 100; i++)
        {
            var expmax = (i * 50) + (i * 10 * i);

            if (this.Exp >= expmax && i < 99)
            {
                continue;
            }

            level = i;
            break;
        }
        this.Level = level;
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

        this.WeaponCac = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponCac(0);
        this.WeaponGun = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponGun(0);

        this._inventory.Clear();

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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
        DataTable table;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            table = UserRoleplayItemDao.GetAll(dbClient, this._id, this._rpId);
        }

        foreach (DataRow dataRow in table.Rows)
        {
            if (!this._inventory.ContainsKey(Convert.ToInt32(dataRow["item_id"])))
            {
                _ = this._inventory.TryAdd(Convert.ToInt32(dataRow["item_id"]), new RolePlayInventoryItem(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["item_id"]), Convert.ToInt32(dataRow["count"])));
            }
        }


        this.SendPacket(new LoadInventoryRpComposer(this._inventory));
    }

    internal RolePlayInventoryItem GetInventoryItem(int id)
    {
        if (this._inventory.TryGetValue(id, out var item))
        {
            return item;
        }
        else
        {
            return null;
        }
    }

    internal void AddInventoryItem(int itemId, int count = 1)
    {
        var rpItem = WibboEnvironment.GetGame().GetRoleplayManager().ItemManager.GetItem(itemId);
        if (rpItem == null)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        var item = this.GetInventoryItem(itemId);
        if (item == null)
        {
            var id = UserRoleplayItemDao.Insert(dbClient, this._id, this._rpId, itemId, count);
            _ = this._inventory.TryAdd(itemId, new RolePlayInventoryItem(id, itemId, count));
        }
        else
        {
            item.Count += count;
            UserRoleplayItemDao.UpdateAddCount(dbClient, item.Id, count);
        }

        this.SendPacket(new AddInventoryItemRpComposer(rpItem, count));
    }

    internal void RemoveInventoryItem(int itemId, int count = 1)
    {
        var item = this.GetInventoryItem(itemId);
        if (item == null)
        {
            return;
        }

        if (item.Count > count)
        {
            item.Count -= count;

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserRoleplayItemDao.UpdateRemoveCount(dbClient, item.Id, count);
        }
        else
        {
            _ = this._inventory.TryRemove(itemId, out _);

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserRoleplayItemDao.Delete(dbClient, this._id);
        }

        this.SendPacket(new RemoveItemInventoryRpComposer(itemId, count));
    }

    public void SendPacket(IServerPacket message)
    {
        var session = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this._id);
        session?.SendPacket(message);
    }

    internal void RemoveMunition(int count)
    {
        if (this.Munition - count <= 0)
        {
            this.Munition = 0;
        }
        else
        {
            this.Munition -= count;
        }
    }

    internal void AddMunition(int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (count > 99)
        {
            count = 99;
        }

        if (this.Munition + count > 99)
        {
            this.Munition = 99;
        }
        else
        {
            this.Munition += count;
        }
    }

    internal void AddHealth(int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (this.Health + count > this.HealthMax)
        {
            this.Health = this.HealthMax;
        }
        else
        {
            this.Health += count;
        }
    }

    public void AddExp(int pNb)
    {
        this.Exp += pNb;

        var level = 1;
        for (var i = 1; i < 100; i++)
        {
            var expmax = (i * 50) + (i * 10 * i);

            if (this.Exp >= expmax && i < 99)
            {
                continue;
            }

            level = i;
            break;
        }

        if (this.Level < level)
        {
            this.Level = level;
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

        var level = 1;
        for (var i = 1; i < 100; i++)
        {
            var expmax = (i * 50) + (i * 10 * i);

            if (this.Exp >= expmax && i < 99)
            {
                continue;
            }

            level = i;
            break;
        }

        if (this.Level != level)
        {
            this.Level = level;
            this.HealthMax = 90 + (this.Level * 10);
            this.Health = this.HealthMax;
            this.SendUpdate();
        }
    }

    internal void AddEnergy(int count)
    {
        if (this.Energy + count > 100)
        {
            this.Energy = 100;
        }
        else
        {
            this.Energy += count;
        }
    }

    internal void RemoveEnergy(int nb)
    {
        if (this.Energy - nb < 0)
        {
            this.Energy = 0;
        }
        else
        {
            this.Energy -= nb;
        }
    }

    public void Hit(RoomUser user, int dmg, Room room, bool slow = false, bool whisper = false, bool aggro = true)
    {
        if (this.Dead || this.SendPrison)
        {
            return;
        }

        if (this.Health <= dmg)
        {
            this.Health = 0;
            this.Dead = true;
            this.DeadTimer = 30;

            user.SetStatus("lay", "0.7");
            user.Freeze = true;
            user.FreezeEndCounter = 0;
            user.IsLay = true;
            user.UpdateNeeded = true;

            if (user.Client != null)
            {
                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.userdead", user.Client.Langue));
            }

            if (this.Money > 10)
            {
                var monaiePerdu = (int)Math.Floor((double)(this.Money / 100) * 20);
                this.Money -= monaiePerdu;

                _ = room.RoomItemHandling.AddTempItem(user.VirtualId, 5461, user.SetX, user.SetY, user.Z, "1", monaiePerdu, InteractionTypeTemp.Money);
            }

            if (user.Client != null)
            {
                user.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.chat.ko", user.Client.Langue), this.Health, this.HealthMax), 0, true);
            }
        }
        else
        {
            this.Health -= dmg;
            if (slow)
            {
                if (this.SlowTimer == 0)
                {
                    if (user.Client != null)
                    {
                        user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.hitslow", user.Client.Langue));
                    }
                }
                this.SlowTimer = 6;
            }

            if (aggro)
            {
                this.AggroTimer = 30;
            }

            if (user.Client != null)
            {
                if (whisper)
                {
                    user.OnChatMe(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.hit", user.Client.Langue), this.Health, this.HealthMax, dmg), 0, true);
                }
                else
                {
                    user.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.hit", user.Client.Langue), this.Health, this.HealthMax, dmg), 0, true);
                }
            }
        }

        this.SendUpdate();
    }

    public void SendUpdate(bool sendNow = false)
    {
        if (sendNow)
        {
            this.SendPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Money, this.Munition, this.Level));
        }
        else
        {
            this.NeedUpdate = true;
        }
    }

    public void SendItemsList(List<RPItem> itemsList) => this.SendPacket(new BuyItemsListComposer(itemsList));

    public void OnCycle(RoomUser user, RolePlayerManager rpManager)
    {

        if (this.SlowTimer > 0)
        {
            this.SlowTimer--;

            user.BreakWalkEnable = true;
        }
        else
        {
            user.BreakWalkEnable = false;
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
                    user.Freeze = false;
                    user.IsSit = false;
                    user.RemoveStatus("sit");
                    user.UpdateNeeded = true;
                }
            }
        }
        else
        {
            if (this.Energy <= 0)
            {
                this.PlayerOutTimer = 60;

                user.RotBody = 2;
                user.RotHead = 2;
                user.Freeze = true;
                user.FreezeEndCounter = 0;
                user.OnChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.chat.energyout", user.Client.Langue));
                user.SetStatus("sit", "0.5");
                user.IsSit = true;
                user.UpdateNeeded = true;

                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.energyout", user.Client.Langue), true);
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
                user.OnChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.chat.loadgun", user.Client.Langue));
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
                if (user.Client != null && user.Client.User != null)
                {
                    user.Room.RoomUserManager.RemoveUserFromRoom(user.Client, true, false);

                    if (rpManager.PrisonId > 0 && !user.Client.User.IsTeleporting)
                    {
                        user.Client.User.IsTeleporting = true;
                        user.Client.User.TeleportingRoomID = rpManager.PrisonId;
                        user.Client.SendPacket(new RoomForwardComposer(rpManager.PrisonId));
                    }
                }
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

                if (user.Client != null && user.Client.User != null)
                {
                    user.Room.RoomUserManager.RemoveUserFromRoom(user.Client, true, false);

                    if (rpManager.HopitalId > 0 && !user.Client.User.IsTeleporting)
                    {
                        user.Client.User.IsTeleporting = true;
                        user.Client.User.TeleportingRoomID = rpManager.HopitalId;
                        user.Client.SendPacket(new RoomForwardComposer(rpManager.HopitalId));
                    }
                }
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
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserRoleplayDao.Update(dbClient, this._id, this._rpId, this.Health, this.Energy, this.Money, this.Munition, this.Exp, this.WeaponGun.Id, this.WeaponCac.Id);
        }

        this.SendPacket(new RpStatsComposer(0, 0, 0, 0, 0, 0, 0));
        this._inventory.Clear();
    }
}
