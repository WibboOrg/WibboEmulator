namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal sealed class PlaceBotEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
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

        var botId = packet.PopInt();
        var x = packet.PopInt();
        var y = packet.PopInt();

        if (!room.GameMap.CanWalk(x, y, false) || !room.GameMap.ValidTile(x, y))
        {
            return;
        }

        if (!Session.User.InventoryComponent.TryGetBot(botId, out var bot))
        {
            return;
        }

        if (room.RoomUserManager.BotPetCount >= 30)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.placebot.error", Session.Language));
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            BotUserDao.UpdatePosition(dbClient, bot.Id, room.Id, x, y);
        }

        _ = room.RoomUserManager.DeployBot(new RoomBot(bot.Id, bot.OwnerId, room.Id, bot.AIType, bot.WalkingEnabled, bot.Name, bot.Motto, bot.Gender, bot.Figure, x, y, 0, 2, bot.ChatEnabled, bot.ChatText, bot.ChatSeconds, bot.IsDancing, bot.Enable, bot.Handitem, bot.Status), null);

        if (!Session.User.InventoryComponent.TryRemoveBot(botId, out _))
        {
            return;
        }

        Session.SendPacket(new BotRemovedFromInventoryComposer(botId));
    }
}
