namespace WibboEmulator.Games.Chats.Commands.User.Room;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class HideWireds : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.RoomData.HideWireds = !room.RoomData.HideWireds;

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdateHideWireds(dbClient, room.Id, room.RoomData.HideWireds);
        }

        if (room.RoomData.HideWireds)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.hidewireds.true", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.hidewireds.false", Session.Language));
        }
    }
}
