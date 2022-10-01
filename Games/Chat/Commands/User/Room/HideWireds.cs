using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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
