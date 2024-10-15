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

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
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

        if (!int.TryParse(exchange.ItemData.ItemName.Split('_')[1], out var value))
        {
            return;
        }

        if (value > 0)
        {
            if (exchange.ItemData.ItemName.StartsWith("CF_") || exchange.ItemData.ItemName.StartsWith("CFC_"))
            {
                Session.User.Credits += value;
                Session.SendPacket(new CreditBalanceComposer(Session.User.Credits));
            }
            else if (exchange.ItemData.ItemName.StartsWith("PntEx_"))
            {
                Session.User.WibboPoints += value;
                Session.SendPacket(new ActivityPointNotificationComposer(Session.User.WibboPoints, 0, 105));

                UserDao.UpdateAddPoints(dbClient, Session.User.Id, value);
            }
            else if (exchange.ItemData.ItemName.StartsWith("WwnEx_"))
            {
                UserStatsDao.UpdateAchievementScore(dbClient, Session.User.Id, value);

                Session.User.AchievementPoints += value;
                Session.SendPacket(new AchievementScoreComposer(Session.User.AchievementPoints));

                var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
                if (roomUserByUserId != null)
                {
                    Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }
        }
    }
}
