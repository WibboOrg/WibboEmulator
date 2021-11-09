using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class RoomRemoveSell : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session == null || Session.GetHabbo() == null)
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
