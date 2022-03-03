using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.AI;
using Butterfly.Game.Users.Inventory.Bots;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            int BotId = Packet.PopInt();
            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false) || !Room.GetGameMap().ValidTile(X, Y))
            {
                return;
            }

            if (!Session.GetUser().GetInventoryComponent().TryGetBot(BotId, out Bot Bot))
            {
                return;
            }

            if (Room.GetRoomUserManager().BotPetCount >= 30)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.placebot.error", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotUserDao.UpdatePosition(dbClient, Bot.Id, Room.Id, X, Y);
            }

            RoomUser roomUser = Room.GetRoomUserManager().DeployBot(new RoomBot(Bot.Id, Bot.OwnerId, Room.Id, BotAIType.Generic, Bot.WalkingEnabled, Bot.Name, Bot.Motto, Bot.Gender, Bot.Figure, X, Y, 0, 2, Bot.ChatEnabled, Bot.ChatText, Bot.ChatSeconds, Bot.IsDancing, Bot.Enable, Bot.Handitem, Bot.Status), null);

            if (!Session.GetUser().GetInventoryComponent().TryRemoveBot(BotId, out Bot ToRemove))
            {
                return;
            }

            Session.SendPacket(new BotInventoryComposer(Session.GetUser().GetInventoryComponent().GetBots()));
        }
    }
}