namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Inventory.Bots;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Database.Daos.Bot;

internal class PlaceBotEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
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

        var BotId = Packet.PopInt();
        var X = Packet.PopInt();
        var Y = Packet.PopInt();

        if (!room.GetGameMap().CanWalk(X, Y, false) || !room.GetGameMap().ValidTile(X, Y))
        {
            return;
        }

        if (!session.GetUser().GetInventoryComponent().TryGetBot(BotId, out var Bot))
        {
            return;
        }

        if (room.GetRoomUserManager().BotPetCount >= 30)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.placebot.error", session.Langue));
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotUserDao.UpdatePosition(dbClient, Bot.Id, room.Id, X, Y);
        }

        var roomUser = room.GetRoomUserManager().DeployBot(new RoomBot(Bot.Id, Bot.OwnerId, room.Id, BotAIType.Generic, Bot.WalkingEnabled, Bot.Name, Bot.Motto, Bot.Gender, Bot.Figure, X, Y, 0, 2, Bot.ChatEnabled, Bot.ChatText, Bot.ChatSeconds, Bot.IsDancing, Bot.Enable, Bot.Handitem, Bot.Status), null);

        if (!session.GetUser().GetInventoryComponent().TryRemoveBot(BotId, out var ToRemove))
        {
            return;
        }

        session.SendPacket(new BotInventoryComposer(session.GetUser().GetInventoryComponent().GetBots()));
    }
}
