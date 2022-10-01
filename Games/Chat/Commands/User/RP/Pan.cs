using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Roleplay.Player;
using WibboEmulator.Game.Rooms.Map.Movement;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class Pan : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!Room.IsRoleplay || UserRoom.Freeze)
            {
                return;
            }

            if(!Room.Roleplay.Pvp)
            {
                return;
            }

            RolePlayer Rp = UserRoom.Roleplayer;
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
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("rp.munitionnotfound", Session.Langue));
                return;
            }

            if (Rp.GunLoad <= 0)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("rp.reloadweapon", Session.Langue));
                return;
            }

            MovementDirection movement = MovementUtility.GetMovementByDirection(UserRoom.RotBody);

            int WeaponEanble = Rp.WeaponGun.Enable;

            UserRoom.ApplyEffect(WeaponEanble, true);
            UserRoom.TimerResetEffect = Rp.WeaponGun.FreezeTime + 1;

            Rp.AggroTimer = 30;

            if (UserRoom.FreezeEndCounter <= Rp.WeaponGun.FreezeTime)
            {
                UserRoom.Freeze = true;
                UserRoom.FreezeEndCounter = Rp.WeaponGun.FreezeTime;
            }

            for (int i = 0; i < Rp.WeaponGun.FreezeTime; i++)
            {
                if (Rp.Munition <= 0 || Rp.GunLoad <= 0)
                {
                    break;
                }

                Rp.Munition--;
                Rp.GunLoad--;

                int Dmg = WibboEnvironment.GetRandomNumber(Rp.WeaponGun.DmgMin, Rp.WeaponGun.DmgMax);
                Room.GetProjectileManager().AddProjectile(UserRoom.VirtualId, UserRoom.SetX, UserRoom.SetY, UserRoom.SetZ, movement, Dmg, Rp.WeaponGun.Distance);
            }

            Rp.SendUpdate();
        }
    }
}
