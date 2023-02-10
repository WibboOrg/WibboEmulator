namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class CreditFurniRedeemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var exchange = room.RoomItemHandling.GetItem(packet.PopInt());
        if (exchange == null)
        {
            return;
        }

        if (exchange.Data.InteractionType != InteractionType.EXCHANGE)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ItemDao.Delete(dbClient, exchange.Id);

        room.RoomItemHandling.RemoveFurniture(null, exchange.Id);

        var value = int.Parse(exchange.GetBaseItem().ItemName.Split(new char[1] { '_' })[1]);

        if (value > 0)
        {
            if (exchange.GetBaseItem().ItemName.StartsWith("CF_") || exchange.GetBaseItem().ItemName.StartsWith("CFC_"))
            {
                session.User.Credits += value;
                session.SendPacket(new CreditBalanceComposer(session.User.Credits));
            }
            else if (exchange.GetBaseItem().ItemName.StartsWith("PntEx_"))
            {
                session.User.WibboPoints += value;
                session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

                UserDao.UpdateAddPoints(dbClient, session.User.Id, value);
            }
            else if (exchange.GetBaseItem().ItemName.StartsWith("WwnEx_"))
            {
                UserStatsDao.UpdateAchievementScore(dbClient, session.User.Id, value);

                session.User.AchievementPoints += value;
                session.SendPacket(new AchievementScoreComposer(session.User.AchievementPoints));

                var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
                if (roomUserByUserId != null)
                {
                    session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }
        }
    }
}
