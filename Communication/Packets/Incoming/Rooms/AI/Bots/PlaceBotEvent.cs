using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Users.Inventory.Bots;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PlaceBotEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
            {
                return;
            }

            int BotId = Packet.PopInt();
            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!room.GetGameMap().CanWalk(X, Y, false) || !room.GetGameMap().ValidTile(X, Y))
            {
                return;
            }

            if (!Session.GetUser().GetInventoryComponent().TryGetBot(BotId, out Bot Bot))
            {
                return;
            }

            if (room.GetRoomUserManager().BotPetCount >= 30)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.placebot.error", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotUserDao.UpdatePosition(dbClient, Bot.Id, room.Id, X, Y);
            }

            RoomUser roomUser = room.GetRoomUserManager().DeployBot(new RoomBot(Bot.Id, Bot.OwnerId, room.Id, BotAIType.Generic, Bot.WalkingEnabled, Bot.Name, Bot.Motto, Bot.Gender, Bot.Figure, X, Y, 0, 2, Bot.ChatEnabled, Bot.ChatText, Bot.ChatSeconds, Bot.IsDancing, Bot.Enable, Bot.Handitem, Bot.Status), null);

            if (!Session.GetUser().GetInventoryComponent().TryRemoveBot(BotId, out Bot ToRemove))
            {
                return;
            }

            Session.SendPacket(new BotInventoryComposer(Session.GetUser().GetInventoryComponent().GetBots()));
        }
    }
}