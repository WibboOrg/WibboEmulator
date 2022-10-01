using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class CreditFurniRedeemEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            Item Exchange = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Exchange == null)
            {
                return;
            }

            if (Exchange.Data.InteractionType != InteractionType.EXCHANGE)
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.Delete(dbClient, Exchange.Id);
            }

            Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id);

            int Value = int.Parse(Exchange.GetBaseItem().ItemName.Split(new char[1] { '_' })[1]);

            if (Value > 0)
            {
                if (Exchange.GetBaseItem().ItemName.StartsWith("CF_") || Exchange.GetBaseItem().ItemName.StartsWith("CFC_"))
                {
                    Session.GetUser().Credits += Value;
                    Session.SendPacket(new CreditBalanceComposer(Session.GetUser().Credits));
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("PntEx_"))
                {
                    Session.GetUser().WibboPoints += Value;
                    Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().WibboPoints, 0, 105));

                    using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    UserDao.UpdateAddPoints(dbClient, Session.GetUser().Id, Value);
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("WwnEx_"))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserStatsDao.UpdateAchievementScore(dbClient, Session.GetUser().Id, Value);
                    }

                    Session.GetUser().AchievementPoints += Value;
                    Session.SendPacket(new AchievementScoreComposer(Session.GetUser().AchievementPoints));

                    RoomUser roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                    if (roomUserByUserId != null)
                    {
                         Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                         Room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                    }
                }
            }
        }
    }
}
