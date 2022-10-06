namespace WibboEmulator.Games.Chat.Commands.User.RP;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Cac : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!Room.IsRoleplay)
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

        if (Rp.Dead || !Rp.PvpEnable || Rp.SendPrison || UserRoom.Freeze)
        {
            return;
        }

        var WeaponEanble = Rp.WeaponCac.Enable;

        UserRoom.ApplyEffect(WeaponEanble, true);
        UserRoom.TimerResetEffect = Rp.WeaponCac.FreezeTime + 1;

        if (UserRoom.FreezeEndCounter <= Rp.WeaponCac.FreezeTime)
        {
            UserRoom.Freeze = true;
            UserRoom.FreezeEndCounter = Rp.WeaponCac.FreezeTime;
        }

        var TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(parameters[1].ToString());

        if (TargetRoomUser == null)
        {
            var BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(parameters[1].ToString());
            if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
            {
                return;
            }

            if (BotOrPet.BotData.RoleBot.Dead)
            {
                return;
            }

            if (Math.Abs(BotOrPet.X - UserRoom.X) >= 2 || Math.Abs(BotOrPet.Y - UserRoom.Y) >= 2)
            {
                return;
            }

            var Dmg = WibboEnvironment.GetRandomNumber(Rp.WeaponCac.DmgMin, Rp.WeaponCac.DmgMax);
            BotOrPet.BotData.RoleBot.Hit(BotOrPet, Dmg, Room, UserRoom.VirtualId, -1);

        }
        else
        {
            var RpTwo = TargetRoomUser.Roleplayer;
            if (RpTwo == null || (!RpTwo.PvpEnable && RpTwo.AggroTimer <= 0))
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetUser().Id == session.GetUser().Id)
            {
                return;
            }

            if (RpTwo.Dead || RpTwo.SendPrison)
            {
                return;
            }

            if (Math.Abs(TargetRoomUser.X - UserRoom.X) >= 2 || Math.Abs(TargetRoomUser.Y - UserRoom.Y) >= 2)
            {
                return;
            }

            var Dmg = WibboEnvironment.GetRandomNumber(Rp.WeaponCac.DmgMin, Rp.WeaponCac.DmgMax);

            Rp.AggroTimer = 30;
            RpTwo.Hit(TargetRoomUser, Dmg, Room);
        }
    }
}
