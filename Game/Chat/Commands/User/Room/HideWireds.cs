using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateHideWireds(dbClient, currentRoom.Id, currentRoom.RoomData.HideWireds);
            }

            if (currentRoom.RoomData.HideWireds)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", Session.Langue));
            }
            else
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", Session.Langue));
            }
        }
    }
}
