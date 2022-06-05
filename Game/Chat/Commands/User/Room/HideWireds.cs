using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class HideWireds : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }

            currentRoom.RoomData.HideWireds = !currentRoom.RoomData.HideWireds;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateHideWireds(dbClient, currentRoom.Id, currentRoom.RoomData.HideWireds);
            }

            if (currentRoom.RoomData.HideWireds)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", Session.Langue));
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", Session.Langue));
            }
        }
    }
}
