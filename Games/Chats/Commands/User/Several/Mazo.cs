namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Mazo : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (Session.User == null)
        {
            return;
        }

        if (room.IsRoleplay)
        {
            return;
        }

        var numberRandom = WibboEnvironment.GetRandomNumber(1, 3);
        var user = Session.User;

        if (numberRandom != 1)
        {
            user.Mazo += 1;

            if (user.MazoHighScore < user.Mazo)
            {
                Session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.mazo.newscore", Session.Language), user.Mazo));
                user.MazoHighScore = user.Mazo;

                using var dbClient = DatabaseManager.Connection;
                UserDao.UpdateMazoScore(dbClient, user.Id, user.MazoHighScore);
            }
            else
            {
                Session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.mazo.win", Session.Language), user.Mazo));
            }

            userRoom.ApplyEffect(566, true);
            userRoom.TimerResetEffect = 4;

        }
        else
        {
            if (user.Mazo > 0)
            {
                Session.SendWhisper(LanguageManager.TryGetValue("cmd.mazo.bigloose", Session.Language));
            }
            else
            {
                Session.SendWhisper(LanguageManager.TryGetValue("cmd.mazo.loose", Session.Language));
            }

            user.Mazo = 0;

            userRoom.ApplyEffect(567, true);
            userRoom.TimerResetEffect = 4;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserDao.UpdateMazo(dbClient, user.Id, user.Mazo);
        }
    }
}
