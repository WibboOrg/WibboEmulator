namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class CreditFurniRedeemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var itemId = packet.PopInt();

        var exchange = room.RoomItemHandling.GetItem(itemId);
        if (exchange == null)
        {
            return;
        }

        if (exchange.Data.InteractionType != InteractionType.EXCHANGE)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;
        ItemDao.DeleteById(dbClient, exchange.Id);

        room.RoomItemHandling.RemoveFurniture(null, exchange.Id);

        var itemName = exchange.ItemData.ItemName;

        var isParsed = itemName.StartsWith("nft_credit_")
            ? int.TryParse(itemName.Split("nft_credit_")[1], out var magotCount)
            : int.TryParse(itemName.Split('_')[1], out magotCount);

        if (!isParsed || magotCount <= 0)
        {
            return;
        }

        if (exchange.ItemData.ItemName.StartsWith("CF_") || exchange.ItemData.ItemName.StartsWith("CFC_") || exchange.ItemData.ItemName.StartsWith("nft_credit_"))
        {
            session.User.Credits += magotCount;
            session.SendPacket(new CreditBalanceComposer(session.User.Credits));
        }
        else if (exchange.ItemData.ItemName.StartsWith("PntEx_"))
        {
            session.User.WibboPoints += magotCount;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

            UserDao.UpdateAddPoints(dbClient, session.User.Id, magotCount);
        }
        else if (exchange.ItemData.ItemName.StartsWith("WwnEx_"))
        {
            UserStatsDao.UpdateAchievementScore(dbClient, session.User.Id, magotCount);

            session.User.AchievementPoints += magotCount;
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
