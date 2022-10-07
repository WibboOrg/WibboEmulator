namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Database.Daos.Bot;

internal class PlaceBotEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
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

        var botId = packet.PopInt();
        var x = packet.PopInt();
        var y = packet.PopInt();

        if (!room.GetGameMap().CanWalk(x, y, false) || !room.GetGameMap().ValidTile(x, y))
        {
            return;
        }

        if (!session.GetUser().GetInventoryComponent().TryGetBot(botId, out var bot))
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
            BotUserDao.UpdatePosition(dbClient, bot.Id, room.Id, x, y);
        }

        _ = room.GetRoomUserManager().DeployBot(new RoomBot(bot.Id, bot.OwnerId, room.Id, BotAIType.Generic, bot.WalkingEnabled, bot.Name, bot.Motto, bot.Gender, bot.Figure, x, y, 0, 2, bot.ChatEnabled, bot.ChatText, bot.ChatSeconds, bot.IsDancing, bot.Enable, bot.Handitem, bot.Status), null);

        if (!session.GetUser().GetInventoryComponent().TryRemoveBot(botId, out _))
        {
            return;
        }

        session.SendPacket(new BotInventoryComposer(session.GetUser().GetInventoryComponent().GetBots()));
    }
}
