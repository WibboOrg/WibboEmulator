using Wibbo.Communication.Packets.Outgoing.Inventory.Bots;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;
using Wibbo.Game.Rooms.AI;
using Wibbo.Game.Users.Inventory.Bots;

namespace Wibbo.Communication.Packets.Incoming.Structure
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

            Room Room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.placebot.error", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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