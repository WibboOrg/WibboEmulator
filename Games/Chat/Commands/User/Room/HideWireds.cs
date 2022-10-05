namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class HideWireds : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
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
