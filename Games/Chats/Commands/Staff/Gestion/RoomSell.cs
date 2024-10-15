namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomSell : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.1", Session.Language));
            return;
        }

        if (!int.TryParse(parameters[1], out var price))
        {
            Session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.2", Session.Language));
            return;
        }
        if (price < 1)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.3", Session.Language));
            return;
        }
        if (price > 99999999)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.4", Session.Language));
            return;
        }

        if (room.RoomData.Group != null)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.5", Session.Language));
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("roomsell.error.6", Session.Language));
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdatePrice(dbClient, room.Id, price);
        }

        room.RoomData.SellPrice = price;

        Session.SendWhisper(string.Format(LanguageManager.TryGetValue("roomsell.valide", Session.Language), price));

        foreach (var user in room.RoomUserManager.UserList.ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("roomsell.warn", Session.Language), price));
        }
    }
}
