using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms.Map.Movement;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.munitionnotfound", Session.Langue));
                return;
            }

            if (Rp.GunLoad <= 0)
            {
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.reloadweapon", Session.Langue));
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

                int Dmg = ButterflyEnvironment.GetRandomNumber(Rp.WeaponGun.DmgMin, Rp.WeaponGun.DmgMax);
                Room.GetProjectileManager().AddProjectile(UserRoom.VirtualId, UserRoom.SetX, UserRoom.SetY, UserRoom.SetZ, movement, Dmg, Rp.WeaponGun.Distance);
            }

            Rp.SendUpdate();
        }
    }
}
