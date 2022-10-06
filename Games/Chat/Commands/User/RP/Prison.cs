namespace WibboEmulator.Games.Chat.Commands.User.RP;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Prison : IChatCommand
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

        if (Rp.Dead || Rp.SendPrison)
        {
            return;
        }

        var TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(parameters[1].ToString());

        if (TargetRoomUser == null)
        {
            return;
        }

        var RpTwo = TargetRoomUser.Roleplayer;
        if (RpTwo == null)
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

        if (RpTwo.SendPrison)
        {
            return;
        }

        if (Math.Floor((double)(RpTwo.Health / (double)RpTwo.HealthMax) * 100) > 75)
        {
            UserRoom.OnChat("*Tente d'arrêter " + TargetRoomUser.GetUsername() + "*");
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("rp.prisonnotallowed", session.Langue));
            return;
        }

        if (!(Math.Abs(TargetRoomUser.X - UserRoom.X) >= 2 || Math.Abs(TargetRoomUser.Y - UserRoom.Y) >= 2))
        {
            UserRoom.OnChat("*Arrête et envoie en prison " + TargetRoomUser.GetUsername() + "*");

            TargetRoomUser.ApplyEffect(729, true);
            TargetRoomUser.RotBody = 2;
            TargetRoomUser.RotHead = 2;
            TargetRoomUser.SetStatus("sit", "0.5");
            TargetRoomUser.Freeze = true;
            TargetRoomUser.FreezeEndCounter = 0;
            TargetRoomUser.IsSit = true;
            TargetRoomUser.UpdateNeeded = true;

            RpTwo.SendPrison = true;
            RpTwo.PrisonTimer = 10 * 2;
        }

        //UserRoom.ApplyEffect(737, true);
        //UserRoom.TimerResetEffect = 2;

        if (UserRoom.FreezeEndCounter <= 2)
        {
            UserRoom.Freeze = true;
            UserRoom.FreezeEndCounter = 2;
        }
    }
}
