namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
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

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdatePrice(dbClient, room.Id, 0);
        }

        userRoom.SendWhisperChat(LanguageManager.TryGetValue("roomsell.remove", session.Language));
    }
}
