using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using System.Linq;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RoomSell : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.1", Session.Langue));
                return;
            }

            if (!int.TryParse(Params[1], out int Prix))
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.2", Session.Langue));
                return;
            }
            if (Prix < 1)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.3", Session.Langue));
                return;
            }
            if (Prix > 99999999)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.4", Session.Langue));
                return;
            }

            if (Room.RoomData.Group != null)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.5", Session.Langue));
                return;
            }

            if (Room.RoomData.SellPrice > 0)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.6", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdatePrice(dbClient, Room.Id, Prix);
            }

            Room.RoomData.SellPrice = Prix;

            UserRoom.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.valide", Session.Langue), Prix));

            foreach (RoomUser user in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (user == null || user.IsBot)
                {
                    continue;
                }

                user.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.warn", Session.Langue), Prix));
            }
        }
    }
}
