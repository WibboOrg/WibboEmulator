namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

internal class Pan : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (!Room.IsRoleplay || UserRoom.Freeze)
        {
            return;
        }

        if (!Room.Roleplay.Pvp)
        {
            return;
        }

        var Rp = UserRoom.Roleplayer;
        if (Rp == null)
        {
            return;
        }

        if (Rp.Dead || !Rp.PvpEnable || Rp.SendPrison)
        {
            return;
        }

        if (Rp.Munition <= 0)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("rp.munitionnotfound", session.Langue));
            return;
        }

        if (Rp.GunLoad <= 0)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("rp.reloadweapon", session.Langue));
            return;
        }

        var movement = MovementUtility.GetMovementByDirection(UserRoom.RotBody);

        var WeaponEanble = Rp.WeaponGun.Enable;

        UserRoom.ApplyEffect(WeaponEanble, true);
        UserRoom.TimerResetEffect = Rp.WeaponGun.FreezeTime + 1;

        Rp.AggroTimer = 30;

        if (UserRoom.FreezeEndCounter <= Rp.WeaponGun.FreezeTime)
        {
            UserRoom.Freeze = true;
            UserRoom.FreezeEndCounter = Rp.WeaponGun.FreezeTime;
        }

        for (var i = 0; i < Rp.WeaponGun.FreezeTime; i++)
        {
            if (Rp.Munition <= 0 || Rp.GunLoad <= 0)
            {
                break;
            }

            Rp.Munition--;
            Rp.GunLoad--;

            var Dmg = WibboEnvironment.GetRandomNumber(Rp.WeaponGun.DmgMin, Rp.WeaponGun.DmgMax);
            Room.GetProjectileManager().AddProjectile(UserRoom.VirtualId, UserRoom.SetX, UserRoom.SetY, UserRoom.SetZ, movement, Dmg, Rp.WeaponGun.Distance);
        }

        Rp.SendUpdate();
    }
}
