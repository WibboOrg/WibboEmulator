namespace WibboEmulator.Games.Chat.Commands.User.Premium;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class RandomLook : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame)
        {
            return;
        }

        if (session.GetUser() == null)
        {
            return;
        }

        if (userRoom.IsTransf || userRoom.IsSpectator)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            session.GetUser().Look = UserWardrobeDao.GetOneRandomLook(dbClient);
        }

        session.SendPacket(new FigureUpdateComposer(session.GetUser().Look, session.GetUser().Gender));
        session.SendPacket(new UserChangeComposer(userRoom, true));
        room.SendPacket(new UserChangeComposer(userRoom, false));
    }
}
