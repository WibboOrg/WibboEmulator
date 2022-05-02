using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdatePrice(dbClient, Room.Id, 0);
            }

            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.remove", Session.Langue));
        }
    }
}
