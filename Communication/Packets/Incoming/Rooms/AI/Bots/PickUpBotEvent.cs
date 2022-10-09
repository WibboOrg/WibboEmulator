namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Inventory.Bots;

internal class PickUpBotEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        var botId = packet.PopInt();
        if (botId <= 0)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;
        if (room == null || !room.CheckRights(session, true))
        {
            return;
        }

        if (!room.GetRoomUserManager().TryGetBot(botId, out var botUser))
        {
            var targetUser = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(botId);
            if (targetUser == null)
            {
                return;
            }

            //Check some values first, please!
            if (targetUser.GetClient() == null || targetUser.GetClient().GetUser() == null)
            {
                return;
            }

            targetUser.TransfBot = false;

            room.SendPacket(new UserRemoveComposer(targetUser.VirtualId));

            room.SendPacket(new UsersComposer(targetUser));
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotUserDao.UpdateRoomId(dbClient, botId);
        }

        _ = session.GetUser().GetInventoryComponent().TryAddBot(new Bot(botUser.BotData.Id, botUser.BotData.OwnerId, botUser.BotData.Name, botUser.BotData.Motto, botUser.BotData.Look, botUser.BotData.Gender, botUser.BotData.WalkingEnabled, botUser.BotData.AutomaticChat, botUser.BotData.ChatText, botUser.BotData.SpeakingInterval, botUser.BotData.IsDancing, botUser.BotData.Enable, botUser.BotData.Handitem, botUser.BotData.Status));
        session.SendPacket(new BotInventoryComposer(session.GetUser().GetInventoryComponent().GetBots()));
        room.GetRoomUserManager().RemoveBot(botUser.VirtualId, false);
    }
}
