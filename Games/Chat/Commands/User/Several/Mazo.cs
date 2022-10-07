namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Mazo : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (room.IsRoleplay)
        {
            return;
        }

        var numberRandom = WibboEnvironment.GetRandomNumber(1, 3);
        var user = session.GetUser();

        if (numberRandom != 1)
        {
            user.Mazo += 1;

            if (user.MazoHighScore < user.Mazo)
            {
                session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.newscore", session.Langue), user.Mazo));
                user.MazoHighScore = user.Mazo;

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateMazoScore(dbClient, user.Id, user.MazoHighScore);
            }
            else
            {
                session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.win", session.Langue), user.Mazo));
            }

            userRoom.ApplyEffect(566, true);
            userRoom.TimerResetEffect = 4;

        }
        else
        {
            if (user.Mazo > 0)
            {
                session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.bigloose", session.Langue));
            }
            else
            {
                session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.loose", session.Langue));
            }

            user.Mazo = 0;

            userRoom.ApplyEffect(567, true);
            userRoom.TimerResetEffect = 4;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateMazo(dbClient, user.Id, user.Mazo);
        }
    }
}
