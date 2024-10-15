namespace WibboEmulator.Games.Chats.Commands.User.RP;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

internal sealed class Pan : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (!room.IsRoleplay || userRoom.Freeze)
        {
            return;
        }

        if (!room.RoomRoleplay.Pvp)
        {
            return;
        }

        var rp = userRoom.Roleplayer;
        if (rp == null)
        {
            return;
        }

        if (rp.Dead || !rp.PvpEnable || rp.SendPrison || !rp.FarEnable)
        {
            return;
        }

        if (rp.Munition <= 0)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("rp.munitionnotfound", Session.Language));
            return;
        }

        if (rp.GunLoad <= 0)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("rp.reloadweapon", Session.Language));
            return;
        }

        var movement = MovementUtility.GetMovementByDirection(userRoom.RotBody);

        var weaponEanble = rp.WeaponGun.Enable;

        userRoom.ApplyEffect(weaponEanble, true);
        userRoom.TimerResetEffect = rp.WeaponGun.FreezeTime + 1;

        rp.AggroTimer = 30;

        if (userRoom.FreezeEndCounter <= rp.WeaponGun.FreezeTime)
        {
            userRoom.Freeze = true;
            userRoom.FreezeEndCounter = rp.WeaponGun.FreezeTime;
        }

        for (var i = 0; i < rp.WeaponGun.FreezeTime; i++)
        {
            if (rp.Munition <= 0 || rp.GunLoad <= 0)
            {
                break;
            }

            rp.Munition--;
            rp.GunLoad--;

            var dmg = WibboEnvironment.GetRandomNumber(rp.WeaponGun.DmgMin, rp.WeaponGun.DmgMax);
            room.ProjectileManager.AddProjectile(userRoom.VirtualId, userRoom.SetX, userRoom.SetY, userRoom.SetZ, movement, dmg, rp.WeaponGun.Distance);
        }

        rp.SendUpdate();
    }
}
