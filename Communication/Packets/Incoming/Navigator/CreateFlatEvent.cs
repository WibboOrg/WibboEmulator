namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal sealed class CreateFlatEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var maxRoomCapacity = 200;

        if (session.User.HasPermission("premium_legend"))
        {
            maxRoomCapacity = 400;
        }
        else if (session.User.HasPermission("premium_epic"))
        {
            maxRoomCapacity = 300;
        }
        else if (session.User.HasPermission("premium_classic"))
        {
            maxRoomCapacity = 250;
        }

        if (session.User.UsersRooms.Count >= maxRoomCapacity)
        {
            session.SendPacket(new CanCreateRoomComposer(true, maxRoomCapacity));
            return;
        }

        var name = packet.PopString(100);
        var description = packet.PopString();
        var model = packet.PopString();
        var category = packet.PopInt();
        var maxVisitors = packet.PopInt();
        var tradeSettings = packet.PopInt();

        if (maxVisitors is < 1 or > 50)
        {
            maxVisitors = 10;
        }

        if (tradeSettings is < 0 or > 2)
        {
            tradeSettings = 0;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoomModels(model, out _))
        {
            return;
        }
        else if (name.Length < 3)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.namelengthshort", session.Langue));
            return;
        }

        var roomId = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            roomId = RoomDao.Insert(dbClient, name, description, session.User.Username, model, category, maxVisitors, tradeSettings);
        }
        session.User.UsersRooms.Add(roomId);

        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);

        session.SendPacket(new FlatCreatedComposer(roomData.Id, name));
    }
}
