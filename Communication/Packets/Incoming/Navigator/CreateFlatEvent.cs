using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class CreateFlatEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (Session.GetUser().UsersRooms.Count >= 200)
            {
                Session.SendPacket(new CanCreateRoomComposer(true, 200));
                return;
            }

            string name = Packet.PopString();
            string desc = Packet.PopString();
            string model = Packet.PopString();
            int category = Packet.PopInt();
            int maxVisitors = Packet.PopInt();
            int tradeSettings = Packet.PopInt();

            if (maxVisitors > 50 || maxVisitors < 1)
            {
                maxVisitors = 10;
            }

            if (tradeSettings < 0 || tradeSettings > 2)
            {
                tradeSettings = 0;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoomModels(model, out RoomModel roomModel))
            {
                return;
            }
            else if (name.Length < 3)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.namelengthshort", Session.Langue));
                return;
            }
            else if (name.Length > 200)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.namelengthshort", Session.Langue));
                return;
            }
            else if (desc.Length > 200)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("room.namelengthshort", Session.Langue));
                return;
            }

            int RoomId = 0;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomId = RoomDao.Insert(dbClient, name, desc, Session.GetUser().Username, model, category, maxVisitors, tradeSettings);
            }
            Session.GetUser().UsersRooms.Add(RoomId);

            RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            Session.SendPacket(new FlatCreatedComposer(roomData.Id, name));
        }
    }
}