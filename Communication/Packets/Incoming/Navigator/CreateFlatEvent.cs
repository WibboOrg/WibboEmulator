namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class CreateFlatEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().UsersRooms.Count >= 200)
        {
            session.SendPacket(new CanCreateRoomComposer(true, 200));
            return;
        }

        var name = Packet.PopString();
        var desc = Packet.PopString();
        var model = Packet.PopString();
        var category = Packet.PopInt();
        var maxVisitors = Packet.PopInt();
        var tradeSettings = Packet.PopInt();

        if (maxVisitors is > 50 or < 1)
        {
            maxVisitors = 10;
        }

        if (tradeSettings is < 0 or > 2)
        {
            tradeSettings = 0;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoomModels(model, out var roomModel))
        {
            return;
        }
        else if (name.Length < 3)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.namelengthshort", session.Langue));
            return;
        }
        else if (name.Length > 200)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.namelengthshort", session.Langue));
            return;
        }
        else if (desc.Length > 200)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.namelengthshort", session.Langue));
            return;
        }

        var RoomId = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomId = RoomDao.Insert(dbClient, name, desc, session.GetUser().Username, model, category, maxVisitors, tradeSettings);
        }
        session.GetUser().UsersRooms.Add(RoomId);

        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

        session.SendPacket(new FlatCreatedComposer(roomData.Id, name));
    }
}