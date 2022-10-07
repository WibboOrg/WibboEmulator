namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class CreditFurniRedeemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var exchange = room.GetRoomItemHandler().GetItem(packet.PopInt());
        if (exchange == null)
        {
            return;
        }

        if (exchange.Data.InteractionType != InteractionType.EXCHANGE)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.Delete(dbClient, exchange.Id);
        }

        room.GetRoomItemHandler().RemoveFurniture(null, exchange.Id);

        var value = int.Parse(exchange.GetBaseItem().ItemName.Split(new char[1] { '_' })[1]);

        if (value > 0)
        {
            if (exchange.GetBaseItem().ItemName.StartsWith("CF_") || exchange.GetBaseItem().ItemName.StartsWith("CFC_"))
            {
                session.GetUser().Credits += value;
                session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));
            }
            else if (exchange.GetBaseItem().ItemName.StartsWith("PntEx_"))
            {
                session.GetUser().WibboPoints += value;
                session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().WibboPoints, 0, 105));

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateAddPoints(dbClient, session.GetUser().Id, value);
            }
            else if (exchange.GetBaseItem().ItemName.StartsWith("WwnEx_"))
            {
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserStatsDao.UpdateAchievementScore(dbClient, session.GetUser().Id, value);
                }

                session.GetUser().AchievementPoints += value;
                session.SendPacket(new AchievementScoreComposer(session.GetUser().AchievementPoints));

                var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
                if (roomUserByUserId != null)
                {
                    session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }
        }
    }
}
