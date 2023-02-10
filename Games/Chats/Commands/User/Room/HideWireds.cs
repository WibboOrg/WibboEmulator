namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class HideWireds : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.RoomData.HideWireds = !room.RoomData.HideWireds;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateHideWireds(dbClient, room.Id, room.RoomData.HideWireds);
        }

        if (room.RoomData.HideWireds)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", session.Langue));
        }
    }
}
