using System.Data;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Test : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            DataTable table = RoomDao.GetAllId(dbClient);
            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                int roomId = Convert.ToInt32(dataRow["id"]);

                WibboEnvironment.GetGame().GetRoomManager().LoadRoom(roomId);
            }

            return;
        }
    }
}
