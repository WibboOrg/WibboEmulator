namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class HideWireds : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.Data.HideWireds = !room.Data.HideWireds;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateHideWireds(dbClient, room.Id, room.Data.HideWireds);
        }

        if (room.Data.HideWireds)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", session.Langue));
        }
    }
}
