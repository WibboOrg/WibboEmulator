namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomSell : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.1", session.Language));
            return;
        }

        if (!int.TryParse(parameters[1], out var price))
        {
            session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.2", session.Language));
            return;
        }
        if (price < 1)
        {
            session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.3", session.Language));
            return;
        }
        if (price > 99999999)
        {
            session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.4", session.Language));
            return;
        }

        if (room.RoomData.Group != null)
        {
            session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.5", session.Language));
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.6", session.Language));
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdatePrice(dbClient, room.Id, price);
        }

        room.RoomData.SellPrice = price;

        session.SendWhisper(string.Format(LanguageManager.TryGetValue("roomsell.valide", session.Language), price));

        foreach (var user in room.RoomUserManager.UserList.ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("roomsell.warn", session.Language), price));
        }
    }
}
