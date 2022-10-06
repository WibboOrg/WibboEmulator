namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class CreditFurniRedeemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var Room))
        {
            return;
        }

        if (!Room.CheckRights(session, true))
        {
            return;
        }

        var Exchange = Room.GetRoomItemHandler().GetItem(packet.PopInt());
        if (Exchange == null)
        {
            return;
        }

        if (Exchange.Data.InteractionType != InteractionType.EXCHANGE)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.Delete(dbClient, Exchange.Id);
        }

        Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id);

        var Value = int.Parse(Exchange.GetBaseItem().ItemName.Split(new char[1] { '_' })[1]);

        if (Value > 0)
        {
            if (Exchange.GetBaseItem().ItemName.StartsWith("CF_") || Exchange.GetBaseItem().ItemName.StartsWith("CFC_"))
            {
                session.GetUser().Credits += Value;
                session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));
            }
            else if (Exchange.GetBaseItem().ItemName.StartsWith("PntEx_"))
            {
                session.GetUser().WibboPoints += Value;
                session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().WibboPoints, 0, 105));

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateAddPoints(dbClient, session.GetUser().Id, Value);
            }
            else if (Exchange.GetBaseItem().ItemName.StartsWith("WwnEx_"))
            {
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserStatsDao.UpdateAchievementScore(dbClient, session.GetUser().Id, Value);
                }

                session.GetUser().AchievementPoints += Value;
                session.SendPacket(new AchievementScoreComposer(session.GetUser().AchievementPoints));

                var roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
                if (roomUserByUserId != null)
                {
                    session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    Room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }
        }
    }
}
