namespace WibboEmulator.Games.Chats.Commands.User.Room;

using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class WiredLimit : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.WiredHandler.SecurityEnabled = !room.WiredHandler.SecurityEnabled;

        if (room.WiredHandler.SecurityEnabled)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.wiredlimit.true", session.Langue));
        }
        else
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.wiredlimit.false", session.Langue));
        }

        room.RoomData.WiredSecurity = room.WiredHandler.SecurityEnabled;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateWiredSecurity(dbClient, room.Id, room.RoomData.WiredSecurity);
        }
    }
}
