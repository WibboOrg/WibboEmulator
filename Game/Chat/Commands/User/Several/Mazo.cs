using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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

            int numberRandom = ButterflyEnvironment.GetRandomNumber(1, 3);
            User user = Session.GetUser();

            if (numberRandom != 1)
            {
                user.Mazo += 1;

                if (user.MazoHighScore < user.Mazo)
                {
                    Session.SendWhisper(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.newscore", Session.Langue), user.Mazo));
                    user.MazoHighScore = user.Mazo;

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserDao.UpdateMazoScore(dbClient, user.Id, user.MazoHighScore);
                    }
                }
                else
                {
                    Session.SendWhisper(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.win", Session.Langue), user.Mazo));
                }

                UserRoom.ApplyEffect(566, true);
                UserRoom.TimerResetEffect = 4;

            }
            else
            {
                if (user.Mazo > 0)
                {
                    Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.bigloose", Session.Langue));
                }
                else
                {
                    Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.mazo.loose", Session.Langue));
                }

                user.Mazo = 0;

                UserRoom.ApplyEffect(567, true);
                UserRoom.TimerResetEffect = 4;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateMazo(dbClient, user.Id, user.Mazo);
            }
        }
    }
}
