namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomRemoveSell : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        if (Room.RoomData.SellPrice == 0)
        {
            return;
        }

        Room.RoomData.SellPrice = 0;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdatePrice(dbClient, Room.Id, 0);
        }

        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.remove", session.Langue));
    }
}
