using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class HideWireds : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.RoomData.HideWireds = !Room.RoomData.HideWireds;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateHideWireds(dbClient, Room.Id, Room.RoomData.HideWireds);
            }

            if (Room.RoomData.HideWireds)
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
