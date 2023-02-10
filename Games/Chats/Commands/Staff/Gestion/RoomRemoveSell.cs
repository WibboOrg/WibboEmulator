namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomRemoveSell : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        if (room.RoomData.SellPrice == 0)
        {
            return;
        }

        room.RoomData.SellPrice = 0;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdatePrice(dbClient, room.Id, 0);
        }

        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.remove", session.Langue));
    }
}
