namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class CreateFlatEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        var maxRoomCapacity = 200;

        if (Session.User.HasPermission("premium_legend"))
        {
            maxRoomCapacity = 400;
        }
        else if (Session.User.HasPermission("premium_epic"))
        {
            maxRoomCapacity = 300;
        }
        else if (Session.User.HasPermission("premium_classic"))
        {
            maxRoomCapacity = 250;
        }

        if (Session.User.UsersRooms.Count >= maxRoomCapacity)
        {
            Session.SendPacket(new CanCreateRoomComposer(true, maxRoomCapacity));
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

        if (!RoomManager.TryGetRoomModels(model, out _))
        {
            return;
        }
        else if (name.Length < 3)
        {
            Session.SendNotification(LanguageManager.TryGetValue("room.namelengthshort", Session.Language));
            return;
        }

        var roomId = 0;
        using (var dbClient = DatabaseManager.Connection)
        {
            roomId = RoomDao.Insert(dbClient, name, description, Session.User.Username, model, category, maxVisitors, tradeSettings);
        }
        Session.User.UsersRooms.Add(roomId);

        var roomData = RoomManager.GenerateRoomData(roomId);

        Session.SendPacket(new FlatCreatedComposer(roomData.Id, name));
    }
}
