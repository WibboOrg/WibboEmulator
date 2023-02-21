namespace WibboEmulator.Games.Chats.Commands.User.Room;

using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class WiredLimit : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.WiredHandler.SecurityEnabled = !room.WiredHandler.SecurityEnabled;

        if (!room.WiredHandler.SecurityEnabled)
        {
            userRoom.SendWhisperChat("Attention tu as désactivé la sécurité des wireds. Une mauvaise utilisation de cette commande pourrait détruire ton appartement");
        }
        else
        {
            userRoom.SendWhisperChat("La sécurité des wireds a été activée");
        }

        room.RoomData.WiredSecurity = room.WiredHandler.SecurityEnabled;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateWiredSecurity(dbClient, room.Id, room.RoomData.WiredSecurity);
        }
    }
}
