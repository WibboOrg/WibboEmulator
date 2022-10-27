namespace WibboEmulator.Games.Roleplays.Enemy;
using System.Drawing;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Roleplays.Weapon;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Games.Rooms.PathFinding;

public class RoleBot
{
    public int Health { get; set; }
    public RPWeapon WeaponGun { get; set; }
    public RPWeapon WeaponCac { get; set; }
    public bool Dead { get; set; }
    public int DeadTimer { get; set; }
    public int AggroVirtuelId { get; set; }
    public bool ResetBot { get; set; }
    public int ResetBotTimer { get; set; }
    public int SlowTimer { get; set; }
    public bool Dodge { get; set; }
    public int DodgeTimer { get; set; }
    public int GunCharger { get; set; }
    public int GunLoadTimer { get; set; }
    public int HitCount { get; set; }
    public int ActionTimer { get; set; }
    public int AggroTimer { get; set; }
    public int DodgeStartCount { get; set; }
    public RPEnemy Config { get; set; }

    public RoleBot(RPEnemy enemyConfig)
    {
        this.SetConfig(enemyConfig);

        this.Dead = false;
        this.AggroVirtuelId = 0;
        this.AggroTimer = 0;
        this.ResetBot = false;
        this.ResetBotTimer = 0;
        this.HitCount = 0;
        this.Dodge = false;
        this.DodgeTimer = 0;
        this.GunCharger = 6;
        this.GunLoadTimer = 0;
        this.DodgeStartCount = WibboEnvironment.GetRandomNumber(2, 4);
        this.ActionTimer = WibboEnvironment.GetRandomNumber(10, 30);
    }

    public void SetConfig(RPEnemy enemyConfig)
    {
        this.Config = enemyConfig;

        this.Health = this.Config.Health;
        this.WeaponGun = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponGun(this.Config.WeaponGunId);
        this.WeaponCac = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponCac(this.Config.WeaponCacId);
    }

    private bool IsAllowZone(RoomUser bot)
    {
        var botX = bot.SetStep ? bot.SetX : bot.X;
        var botY = bot.SetStep ? bot.SetY : bot.Y;

        if (Math.Abs(botX - bot.BotData.X) > this.Config.ZoneDistance || Math.Abs(botY - bot.BotData.Y) > this.Config.ZoneDistance)
        {
            return false;
        }

        return true;
    }

    private void ReloadGunCycle(RoomUser bot)
    {
        if (this.GunLoadTimer > 0)
        {
            this.GunLoadTimer--;
            if (this.GunLoadTimer == 0)
            {
                this.GunCharger = 6;
            }
        }
        else
        {
            if (this.GunCharger == 0)
            {
                this.GunLoadTimer = 6;
                bot.OnChat("*Recharge mon arme*");
            }
        }
    }

    public void OnCycle(RoomUser bot, Room room)
    {
        if (this.SlowTimer > 0 || this.Config.ZombieMode)
        {
            if (this.SlowTimer > 0)
            {
                this.SlowTimer--;
            }

            if (!bot.BreakWalkEnable)
            {
                bot.BreakWalkEnable = true;
            }
        }
        else
        {
            if (bot.BreakWalkEnable)
            {
                bot.BreakWalkEnable = false;
            }
        }

        this.ReloadGunCycle(bot);

        if (this.AggroVirtuelId > 0)
        {
            this.AggroCycle(bot, room);
        }

        if (this.Config.AggroDistance > 0 && !this.Dead) // && this.AggroVirtuelId == 0
        {
            this.AggroSearch(bot, room);
        }

        if (!this.ResetBot && !this.Dead && this.AggroVirtuelId == 0 && !bot.Freeze)
        {
            this.FreeTimeCycle(bot);
        }

        if (this.ResetBot && !this.Dead && this.AggroVirtuelId == 0)
        {
            this.CheckResetBot(bot, room);
        }

        if (this.Dead)
        {
            this.DeadTimer--;
            if (this.DeadTimer <= 0)
            {
                this.Dead = false;
                this.Health = this.Config.Health;


                bot.RemoveStatus("lay");
                bot.Freeze = false;
                bot.FreezeEndCounter = 0;
                bot.IsLay = false;
                bot.UpdateNeeded = true;
            }
        }
    }

    public void Hit(RoomUser bot, int dmg, Room room, int aggroVId, int teamId)
    {
        if (this.Dead)
        {
            return;
        }

        if (this.Health <= dmg)
        {
            var user = room.RoomUserManager.GetRoomUserByVirtualId(this.AggroVirtuelId);
            if (user != null && !user.IsBot)
            {
                var rp = user.Roleplayer;
                if (rp != null)
                {
                    rp.AddExp(this.Config.Health);
                }
            }

            this.Health = 0;
            this.Dead = true;
            this.DeadTimer = this.Config.DeadTimer;
            this.AggroVirtuelId = 0;
            this.AggroTimer = 0;
            this.ResetBot = false;
            this.ResetBotTimer = 0;
            this.HitCount = 0;
            this.Dodge = false;
            this.DodgeTimer = 0;

            bot.SetStatus("lay", bot.IsPet ? "" : "0.7");
            bot.Freeze = true;
            bot.FreezeEndCounter = 0;
            bot.IsLay = true;
            bot.UpdateNeeded = true;

            if (this.Config.MoneyDrop > 0)
            {
                _ = room.RoomItemHandling.AddTempItem(bot.VirtualId, this.Config.DropScriptId, bot.SetX, bot.SetY, bot.Z, "1", this.Config.MoneyDrop, InteractionTypeTemp.Money);
            }

            if (this.Config.LootItemId > 0)
            {
                var item = WibboEnvironment.GetGame().GetRoleplayManager().ItemManager.GetItem(this.Config.LootItemId);
                if (item != null)
                {
                    _ = room.RoomItemHandling.AddTempItem(bot.VirtualId, 3996, bot.SetX, bot.SetY, bot.Z, item.Name, this.Config.LootItemId, InteractionTypeTemp.RpItem);
                }
            }

            bot.OnChat("A été mis K.O. ! [" + this.Health + "/" + this.Config.Health + "]", bot.IsPet ? 0 : 2, true);
        }
        else
        {
            this.Health -= dmg;
            this.SlowTimer = 6;

            this.ResetBot = false;
            this.ResetBotTimer = 60;

            this.AggroTimer = 0;

            if (teamId == -1 || teamId != this.Config.TeamId)
            {
                this.AggroVirtuelId = aggroVId;
            }

            if (!this.Dodge)
            {
                this.HitCount += 1;
                if (this.HitCount % this.DodgeStartCount == 0)
                {
                    this.Dodge = true;
                    this.DodgeTimer = 3;
                    this.DodgeStartCount = WibboEnvironment.GetRandomNumber(2, 4);
                }
            }

            bot.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.hit", room.RoomData.Langue), this.Health, this.Config.Health, dmg), bot.IsPet ? 0 : 2, true);
        }
    }

    private void Pan(RoomUser bot, Room room)
    {
        var movement = MovementUtility.GetMovementByDirection(bot.RotBody);

        var weaponEanble = this.WeaponGun.Enable;

        bot.ApplyEffect(weaponEanble, true);
        bot.TimerResetEffect = this.WeaponGun.FreezeTime + 1;

        if (bot.FreezeEndCounter <= this.WeaponGun.FreezeTime)
        {
            bot.Freeze = true;
            bot.FreezeEndCounter = this.WeaponGun.FreezeTime;
        }

        for (var i = 0; i < this.WeaponGun.FreezeTime; i++)
        {
            if (this.GunCharger <= 0)
            {
                return;
            }

            this.GunCharger--;
            var dmg = WibboEnvironment.GetRandomNumber(this.WeaponGun.DmgMin, this.WeaponGun.DmgMax);

            room.ProjectileManager.AddProjectile(bot.VirtualId, bot.SetX, bot.SetY, bot.SetZ, movement, dmg, this.WeaponGun.Distance, this.Config.TeamId, true);
        }
    }

    private void Cac(RoomUser bot, Room room, RoomUser user)
    {
        var dmg = WibboEnvironment.GetRandomNumber(this.WeaponCac.DmgMin, this.WeaponCac.DmgMax);

        if (!user.IsBot)
        {
            var rp = user.Roleplayer;
            if (rp != null)
            {
                rp.Hit(user, dmg, room, false, true);
            }
        }
        else
        {
            if (user.BotData.RoleBot != null)
            {
                user.BotData.RoleBot.Hit(user, dmg, room, bot.VirtualId, user.BotData.RoleBot.Config.TeamId);
            }
        }

        var weaponEanble = this.WeaponCac.Enable;

        bot.ApplyEffect(weaponEanble, true);
        bot.TimerResetEffect = this.WeaponCac.FreezeTime + 1;

        if (bot.FreezeEndCounter <= this.WeaponCac.FreezeTime + 1)
        {
            bot.Freeze = true;
            bot.FreezeEndCounter = this.WeaponCac.FreezeTime + 1;
        }
    }

    public void ResetAggro()
    {
        this.AggroVirtuelId = 0;
        this.AggroTimer = 0;
    }

    private void AggroCycle(RoomUser bot, Room room)
    {
        var user = room.RoomUserManager.GetRoomUserByVirtualId(this.AggroVirtuelId);
        if (user == null || this.Dead)
        {
            this.ResetAggro();
            return;
        }

        if (this.AggroTimer > 120)
        {
            this.ResetAggro();
            return;
        }
        else
        {
            this.AggroTimer++;
        }

        if (!user.IsBot)
        {
            var rp = user.Roleplayer;
            if (rp == null)
            {
                this.ResetAggro();
                return;
            }

            if (rp.Dead || (!rp.PvpEnable && rp.AggroTimer <= 0) || rp.SendPrison || !rp.PvpEnable)
            {
                this.ResetAggro();
                return;
            }
        }
        else
        {
            if (user.BotData.RoleBot == null || user.BotData.RoleBot.Dead)
            {
                this.ResetAggro();
                return;
            }
        }

        var botX = bot.SetStep ? bot.SetX : bot.X;
        var botY = bot.SetStep ? bot.SetY : bot.Y;

        var userX = user.SetStep ? user.SetX : user.X;
        var userY = user.SetStep ? user.SetY : user.Y;

        var distanceX = Math.Abs(userX - botX);
        var distanceY = Math.Abs(userY - botY);

        if (distanceX > this.Config.LostAggroDistance || distanceY > this.Config.LostAggroDistance)
        {
            this.ResetAggro();
            return;
        }

        if (distanceX > this.Config.LostAggroDistance || distanceY > this.Config.LostAggroDistance)
        {
            this.ResetAggro();
            return;
        }

        if (distanceX > this.Config.LostAggroDistance || distanceY > this.Config.LostAggroDistance)
        {
            this.ResetAggro();
            return;
        }

        if (Math.Abs(botX - bot.BotData.X) > this.Config.ZoneDistance + 10 || Math.Abs(botY - bot.BotData.Y) > this.Config.ZoneDistance + 10)
        {
            this.ResetAggro();
            return;
        }

        if (bot.Freeze)
        {
            return;
        }

        if (!this.BotPathFind(bot, room, user))
        {
            return;
        }

        var rot = Rotation.Calculate(botX, botY, userX, userY);

        bot.RotHead = rot;
        bot.RotBody = rot;
        bot.UpdateNeeded = true;

        this.AggroTimer = 0;

        if (this.WeaponCac.Id != 0 && distanceX < 2 && distanceY < 2)
        {
            this.Cac(bot, room, user);
        }
        else if (this.WeaponGun.Id != 0)
        {
            this.Pan(bot, room);
        }
    }

    private bool BotPathFind(RoomUser bot, Room room, RoomUser user)
    {
        var botX = bot.SetStep ? bot.SetX : bot.X;
        var botY = bot.SetStep ? bot.SetY : bot.Y;

        var userX = user.SetStep ? user.SetX : user.X;
        var userY = user.SetStep ? user.SetY : user.Y;

        var distanceX = Math.Abs(userX - botX);
        var distanceY = Math.Abs(userY - botY);

        if (this.Dodge)
        {
            this.DodgeTimer--;
            if (this.DodgeTimer <= 0)
            {
                this.Dodge = false;
                this.HitCount = 0;
            }

            if (!bot.IsWalking)
            {
                if (distanceX < distanceY)
                {
                    bot.MoveTo(userX + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? 1 : -1), botY, true);
                }
                else
                {
                    bot.MoveTo(botX, userY + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? 1 : -1), true);
                }
            }
            return false;
        }

        if (this.WeaponCac.Id == 0 && (this.WeaponGun.Id == 0 || this.GunCharger == 0)) //Fuite
        {
            if (bot.IsWalking)
            {
                return false;
            }

            if (distanceX > distanceY)
            {
                if (user.X > botX)
                {
                    bot.MoveTo(botX, botY + WibboEnvironment.GetRandomNumber(1, 3), true);
                }
                else
                {
                    bot.MoveTo(botX, botY - WibboEnvironment.GetRandomNumber(1, 3), true);
                }
            }
            else
            {
                if (user.Y > botY)
                {
                    bot.MoveTo(botX - WibboEnvironment.GetRandomNumber(1, 3), botY, true);
                }
                else
                {
                    bot.MoveTo(botX + WibboEnvironment.GetRandomNumber(1, 3), botY, true);
                }
            }
            return false;
        }

        if (distanceX >= 2 || distanceY >= 2 || this.WeaponCac.Id == 0) //Distance
        {
            if ((this.WeaponGun.Id == 0 || this.GunCharger == 0) && this.WeaponCac.Id != 0) //Déplace le bot au cac si il est uniquement cac
            {
                bot.MoveTo(userX + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), userY + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), true);
            }
            else if (this.WeaponGun.Id != 0 && this.GunCharger != 0) //Si le bot a une arme distance
            {
                if ((this.WeaponCac.Id == 0 || this.GunCharger == 0) && ((botX == user.X && botY == user.Y) || (botX == userX && botY == userY))) //Eloigné le bot si l'utilisateur est sur sa case et que le bot n'a pas d'arme cac
                {
                    bot.MoveTo(userX + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -5 : 5), userY + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -5 : 5), true);
                    return false;
                }

                if ((botX + botY) == (userX + userY) ||
                    (botX - botY) == (userX - userY) ||
                    (botX + botY) == (userX + userY) ||
                    (botX - botY) == (userX - userY) ||
                    userX == botX || userY == botY ||
                    userX == botX || userY == botY) //Bot en position de tirer
                {
                    var rot = Rotation.Calculate(botX, botY, userX, userY);

                    if (this.CheckCollisionDir(bot, room, rot, distanceX, distanceY)) //Check si la balle peut passer
                    {
                        bot.MoveTo(userX + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), userY + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), true);
                        return false;
                    }
                }
                else
                {
                    if (distanceX < 3 && distanceY < 3)
                    {
                        bot.MoveTo(userX + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), userY + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), true);
                    }
                    else
                    {
                        if (bot.IsWalking)
                        {
                            return false;
                        }

                        if (distanceX < distanceY)
                        {
                            bot.MoveTo(userX, (distanceY > 5) ? botY - WibboEnvironment.GetRandomNumber(1, 2) : botY + WibboEnvironment.GetRandomNumber(1, 2), true);
                        }
                        else
                        {
                            bot.MoveTo((distanceX > 5) ? botX - WibboEnvironment.GetRandomNumber(1, 2) : botX + WibboEnvironment.GetRandomNumber(1, 2), userY, true);
                        }
                    }
                    return false;
                }
            }
        }

        return true;
    }

    private bool CheckCollisionDir(RoomUser bot, Room room, int rot, int distanceX, int distanceY)
    {
        var botX = bot.SetStep ? bot.SetX : bot.X;
        var botY = bot.SetStep ? bot.SetY : bot.Y;
        var botZ = bot.SetStep ? bot.SetZ : bot.Z;

        if (this.WeaponGun.Distance < distanceX || this.WeaponGun.Distance < distanceY)
        {
            return true;
        }

        if (rot == 2)
        {
            for (var i = 1; i < distanceX; i++)
            {
                if (!room.GameMap.CanStackItem(botX + i, botY, true) || room.GameMap.SqAbsoluteHeight(botX + i, botY) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }
        else if (rot == 6)
        {
            for (var i = 1; i < distanceX; i++)
            {
                if (!room.GameMap.CanStackItem(botX - i, botY, true) || room.GameMap.SqAbsoluteHeight(botX - i, botY) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }
        else if (rot == 4)
        {
            for (var i = 1; i < distanceY; i++)
            {
                if (!room.GameMap.CanStackItem(botX, botY + i, true) || room.GameMap.SqAbsoluteHeight(botX, botY + i) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }
        else if (rot == 0)
        {
            for (var i = 1; i < distanceY; i++)
            {
                if (!room.GameMap.CanStackItem(botX, botY - i, true) || room.GameMap.SqAbsoluteHeight(botX, botY - i) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }
        //diago
        else if (rot == 7)
        {
            for (var i = 1; i < distanceX; i++)
            {
                if (!room.GameMap.CanStackItem(botX - i, botY - i, true) || room.GameMap.SqAbsoluteHeight(botX - i, botY - i) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }
        else if (rot == 1)
        {
            for (var i = 1; i < distanceX; i++)
            {
                if (!room.GameMap.CanStackItem(botX + i, botY - i, true) || room.GameMap.SqAbsoluteHeight(botX + i, botY - i) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }
        else if (rot == 3)
        {
            for (var i = 1; i < distanceX; i++)
            {
                if (!room.GameMap.CanStackItem(botX + i, botY + i, true) || room.GameMap.SqAbsoluteHeight(botX + i, botY + i) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }
        else if (rot == 5)
        {
            for (var i = 1; i < distanceX; i++)
            {
                if (!room.GameMap.CanStackItem(botX - i, botY + i, true) || room.GameMap.SqAbsoluteHeight(botX - i, botY + i) > (botZ + 0.5))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void CheckResetBot(RoomUser bot, Room room)
    {
        if ((this.Config.ResetPosition && (this.IsAllowZone(bot) || !bot.IsWalking) && this.Health == this.Config.Health) || (!this.Config.ResetPosition && this.Health == this.Config.Health))
        {
            this.ResetBotTimer = 0;
        }
        else
        {
            this.ResetBotTimer--;
        }

        this.Health = (this.Health + 25 >= this.Config.Health) ? this.Config.Health : this.Health + 25;

        if (this.ResetBotTimer <= 0)
        {
            this.ResetBot = false;
            this.ResetBotTimer = 0;

            this.Health = this.Config.Health;

            if (this.Config.ResetPosition && !this.IsAllowZone(bot))
            {
                bot.RotHead = bot.BotData.Rot;
                bot.RotBody = bot.BotData.Rot;
                room.SendPacket(RoomItemHandling.TeleportUser(bot, new Point(bot.BotData.X, bot.BotData.Y), 0, room.GameMap.SqAbsoluteHeight(bot.BotData.X, bot.BotData.Y)));
            }
        }
    }

    private void FreeTimeCycle(RoomUser bot)
    {
        if (!this.IsAllowZone(bot) || this.Health != this.Config.Health)
        {
            this.ResetBot = true;
            this.ResetBotTimer = 60;

            if (this.Config.ResetPosition && !this.IsAllowZone(bot))
            {
                bot.MoveTo(bot.BotData.X, bot.BotData.Y);
            }

            if (this.Health != this.Config.Health)
            {
                bot.ApplyEffect(4, true);
                bot.TimerResetEffect = 4;
            }

            return;
        }

        //Action bot
        if (this.ActionTimer > 0)
        {
            this.ActionTimer--;
            return;
        }

        if (bot.IsWalking)
        {
            return;
        }

        this.ActionTimer = WibboEnvironment.GetRandomNumber(15, 30);
        if (this.ActionTimer >= 25 && !this.Config.ZombieMode)
        {
            if (this.ActionTimer == 30)
            {
                if (bot.RotBody != bot.BotData.Rot)
                {
                    bot.RotHead = bot.BotData.Rot;
                    bot.RotBody = bot.BotData.Rot;
                    bot.UpdateNeeded = true;
                }
            }
            else
            {
                if (bot.IsSit)
                {
                    bot.RemoveStatus("sit");
                    bot.IsSit = false;
                    bot.UpdateNeeded = true;
                }
                else
                {
                    if (bot.RotBody % 2 == 0)
                    {
                        if (bot.IsPet)
                        {
                            bot.SetStatus("sit", "0");
                        }
                        else
                        {
                            bot.SetStatus("sit", "0.5");
                        }

                        bot.IsSit = true;
                        bot.UpdateNeeded = true;
                    }
                }
            }
        }
        else
        {
            if (this.Config.ZoneDistance > 0)
            {
                //Bouge le bot aléatoirement dans sa zone
                var lenghtX = WibboEnvironment.GetRandomNumber(0, this.Config.ZoneDistance);
                var lenghtY = WibboEnvironment.GetRandomNumber(0, this.Config.ZoneDistance);
                bot.MoveTo(bot.BotData.X + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -lenghtX : lenghtX), bot.BotData.Y + ((WibboEnvironment.GetRandomNumber(1, 2) == 2) ? -lenghtY : lenghtY), true);
            }
        }
    }

    private void AggroSearch(RoomUser bot, Room room)
    {
        var users = room.GameMap.GetNearUsers(new Point(bot.X, bot.Y), this.Config.AggroDistance);
        if (users == null)
        {
            return;
        }

        var botX = bot.SetStep ? bot.SetX : bot.X;
        var botY = bot.SetStep ? bot.SetY : bot.Y;

        var distanceUserNow = 99;

        foreach (var user in users)
        {
            if (user == bot)
            {
                continue;
            }

            var rotationDistance = Math.Abs(Rotation.Calculate(botX, botY, user.X, user.Y) - bot.RotBody);
            if (rotationDistance >= 2 && !(user.X == botX && user.Y == botY))
            {
                continue;
            }

            if (!user.IsBot)
            {
                var rp = user.Roleplayer;
                if (rp == null)
                {
                    continue;
                }

                if (rp.Dead || (!rp.PvpEnable && rp.AggroTimer <= 0) || rp.SendPrison)
                {
                    continue;
                }
            }
            else
            {
                if (user.BotData.RoleBot == null || user.BotData.RoleBot.Dead || this.Config.TeamId == user.BotData.RoleBot.Config.TeamId)
                {
                    continue;
                }
            }

            var userX = user.SetStep ? user.SetX : user.X;
            var userY = user.SetStep ? user.SetY : user.Y;

            var distanceX = Math.Abs(userX - botX);
            var distanceY = Math.Abs(userY - botY);

            var distanceUser = distanceX + distanceY;

            if (distanceUser >= distanceUserNow)
            {
                continue;
            }

            distanceUserNow = distanceUser;

            this.ResetBot = false;
            this.ResetBotTimer = 60;
            this.AggroVirtuelId = user.VirtualId;
            this.AggroTimer = 0;
        }
    }
}
