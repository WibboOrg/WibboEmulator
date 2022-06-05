using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class RoomRemoveSell : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (Room.RoomData.SellPrice == 0)
            {
                return;
            }

            Room.RoomData.SellPrice = 0;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdatePrice(dbClient, Room.Id, 0);
            }

            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.remove", Session.Langue));
        }
    }
}
