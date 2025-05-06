namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Mazo : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User == null)
        {
            return;
        }

        if (room.IsRoleplay)
        {
            return;
        }

        var timeSpan = DateTime.Now - session.User.MazoTimer;
        if (timeSpan.TotalSeconds < 2)
        {
            session.SendWhisper($"Veuillez patienter pendant {timeSpan.TotalSeconds} secondes avant de pouvoir réutiliser le mazo.");
            return;
        }

        session.User.MazoTimer = DateTime.Now;

        var numberRandom = WibboEnvironment.GetRandomNumber(1, 3);
        var user = session.User;

        if (numberRandom != 1)
        {
            user.Mazo += 1;

            if (user.MazoHighScore < user.Mazo)
            {
                session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.mazo.newscore", session.Language), user.Mazo));
                user.MazoHighScore = user.Mazo;

                using var dbClient = DatabaseManager.Connection;
                UserDao.UpdateMazoScore(dbClient, user.Id, user.MazoHighScore);
            }
            else
            {
                session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.mazo.win", session.Language), user.Mazo));
            }

            userRoom.ApplyEffect(566, true);
            userRoom.TimerResetEffect = 4;
        }
        else
        {
            if (user.Mazo > 0)
            {
                session.SendWhisper(LanguageManager.TryGetValue("cmd.mazo.bigloose", session.Language));
            }
            else
            {
                session.SendWhisper(LanguageManager.TryGetValue("cmd.mazo.loose", session.Language));
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
