using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Users;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class Mazo : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            if (Room.IsRoleplay)
            {
                return;
            }

            int numberRandom = WibboEnvironment.GetRandomNumber(1, 3);
            User user = Session.GetUser();

            if (numberRandom != 1)
            {
                user.Mazo += 1;

                if (user.MazoHighScore < user.Mazo)
                {
                    Session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.newscore", Session.Langue), user.Mazo));
                    user.MazoHighScore = user.Mazo;

                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserDao.UpdateMazoScore(dbClient, user.Id, user.MazoHighScore);
                    }
                }
                else
                {
                    Session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.win", Session.Langue), user.Mazo));
                }

                UserRoom.ApplyEffect(566, true);
                UserRoom.TimerResetEffect = 4;

            }
            else
            {
                if (user.Mazo > 0)
                {
                    Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.bigloose", Session.Langue));
                }
                else
                {
                    Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.loose", Session.Langue));
                }

                user.Mazo = 0;

                UserRoom.ApplyEffect(567, true);
                UserRoom.TimerResetEffect = 4;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateMazo(dbClient, user.Id, user.Mazo);
            }
        }
    }
}
