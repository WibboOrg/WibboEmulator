namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomSell : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.1", session.Langue));
            return;
        }

        if (!int.TryParse(parameters[1], out var price))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.2", session.Langue));
            return;
        }
        if (price < 1)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.3", session.Langue));
            return;
        }
        if (price > 99999999)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.4", session.Langue));
            return;
        }

        if (room.RoomData.Group != null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.5", session.Langue));
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.6", session.Langue));
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            RoomDao.UpdatePrice(dbClient, room.Id, price);
        }

        room.RoomData.SellPrice = price;

        session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.valide", session.Langue), price));

        foreach (var user in room.RoomUserManager.GetUserList().ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.warn", session.Langue), price));
        }
    }
}
