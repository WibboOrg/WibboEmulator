namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class HideWireds : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        Room.RoomData.HideWireds = !Room.RoomData.HideWireds;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateHideWireds(dbClient, Room.Id, Room.RoomData.HideWireds);
        }

        if (Room.RoomData.HideWireds)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", session.Langue));
        }
    }
}
