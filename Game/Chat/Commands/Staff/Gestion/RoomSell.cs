using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class RoomSell : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.1", Session.Langue));
                return;
            }

            if (!int.TryParse(Params[1], out int Prix))
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.2", Session.Langue));
                return;
            }
            if (Prix < 1)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.3", Session.Langue));
                return;
            }
            if (Prix > 99999999)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.4", Session.Langue));
                return;
            }

            if (Room.RoomData.Group != null)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.5", Session.Langue));
                return;
            }

            if (Room.RoomData.SellPrice > 0)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.6", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdatePrice(dbClient, Room.Id, Prix);
            }

            Room.RoomData.SellPrice = Prix;

            Session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.valide", Session.Langue), Prix));

            foreach (RoomUser user in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (user == null || user.IsBot)
                {
                    continue;
                }

                user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.warn", Session.Langue), Prix));
            }
        }
    }
}
