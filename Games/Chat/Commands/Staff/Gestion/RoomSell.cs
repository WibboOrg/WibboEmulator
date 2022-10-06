namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomSell : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.1", session.Langue));
            return;
        }

        if (!int.TryParse(parameters[1], out var Prix))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.2", session.Langue));
            return;
        }
        if (Prix < 1)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.3", session.Langue));
            return;
        }
        if (Prix > 99999999)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.4", session.Langue));
            return;
        }

        if (Room.RoomData.Group != null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.5", session.Langue));
            return;
        }

        if (Room.RoomData.SellPrice > 0)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.6", session.Langue));
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdatePrice(dbClient, Room.Id, Prix);
        }

        Room.RoomData.SellPrice = Prix;

        session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.valide", session.Langue), Prix));

        foreach (var user in Room.GetRoomUserManager().GetUserList().ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.warn", session.Langue), Prix));
        }
    }
}
