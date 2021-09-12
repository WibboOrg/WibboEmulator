using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class RoomRemoveSell : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
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

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE rooms SET price = '0' WHERE id = '" + Room.Id + "' LIMIT 1;");
            }

            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.remove", Session.Langue));
        }
    }
}
