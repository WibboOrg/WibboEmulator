namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Mazo : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (Room.IsRoleplay)
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

            UserRoom.ApplyEffect(566, true);
            UserRoom.TimerResetEffect = 4;

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

            UserRoom.ApplyEffect(567, true);
            UserRoom.TimerResetEffect = 4;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateMazo(dbClient, user.Id, user.Mazo);
        }
    }
}
