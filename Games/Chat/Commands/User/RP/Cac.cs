namespace WibboEmulator.Games.Chat.Commands.User.RP;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Cac : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!room.IsRoleplay)
        {
            return;
        }

        if (!room.Roleplay.Pvp)
        {
            return;
        }

        var rp = userRoom.Roleplayer;
        if (rp == null)
        {
            return;
        }

        if (rp.Dead || !rp.PvpEnable || rp.SendPrison || userRoom.Freeze)
        {
            return;
        }

        var weaponEanble = rp.WeaponCac.Enable;

        userRoom.ApplyEffect(weaponEanble, true);
        userRoom.TimerResetEffect = rp.WeaponCac.FreezeTime + 1;

        if (userRoom.FreezeEndCounter <= rp.WeaponCac.FreezeTime)
        {
            userRoom.Freeze = true;
            userRoom.FreezeEndCounter = rp.WeaponCac.FreezeTime;
        }

        var targetRoomUser = room.GetRoomUserManager().GetRoomUserByName(parameters[1].ToString());

        if (targetRoomUser == null)
        {
            var botOrPet = room.GetRoomUserManager().GetBotOrPetByName(parameters[1].ToString());
            if (botOrPet == null || botOrPet.BotData == null || botOrPet.BotData.RoleBot == null)
            {
                return;
            }

            if (botOrPet.BotData.RoleBot.Dead)
            {
                return;
            }

            if (Math.Abs(botOrPet.X - userRoom.X) >= 2 || Math.Abs(botOrPet.Y - userRoom.Y) >= 2)
            {
                return;
            }

            var dmg = WibboEnvironment.GetRandomNumber(rp.WeaponCac.DmgMin, rp.WeaponCac.DmgMax);
            botOrPet.BotData.RoleBot.Hit(botOrPet, dmg, room, userRoom.VirtualId, -1);

        }
        else
        {
            var rpTwo = targetRoomUser.Roleplayer;
            if (rpTwo == null || (!rpTwo.PvpEnable && rpTwo.AggroTimer <= 0))
            {
                return;
            }

            if (targetRoomUser.GetClient().GetUser().Id == session.GetUser().Id)
            {
                return;
            }

            if (rpTwo.Dead || rpTwo.SendPrison)
            {
                return;
            }

            if (Math.Abs(targetRoomUser.X - userRoom.X) >= 2 || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2)
            {
                return;
            }

            var dmg = WibboEnvironment.GetRandomNumber(rp.WeaponCac.DmgMin, rp.WeaponCac.DmgMax);

            rp.AggroTimer = 30;
            rpTwo.Hit(targetRoomUser, dmg, room);
        }
    }
}
