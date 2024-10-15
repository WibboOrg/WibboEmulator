namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Inventory.Bots;

internal sealed class PickUpBotEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        var botId = packet.PopInt();
        if (botId <= 0)
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null || !room.CheckRights(Session, true))
        {
            return;
        }

        if (!room.RoomUserManager.TryGetBot(botId, out var botUser))
        {
            var TargetUser = Session.User.Room.RoomUserManager.GetRoomUserByUserId(botId);
            if (TargetUser == null)
            {
                return;
            }

            //Check some values first, please!
            if (TargetUser.Client == null || TargetUser.Client.User == null)
            {
                return;
            }

            TargetUser.TransfBot = false;

            room.SendPacket(new UserRemoveComposer(TargetUser.VirtualId));

            room.SendPacket(new UsersComposer(TargetUser));
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            BotUserDao.UpdateRoomId(dbClient, botId);
        }

        _ = Session.User.InventoryComponent.TryAddBot(new Bot(botUser.BotData.Id, botUser.BotData.OwnerId, botUser.BotData.Name, botUser.BotData.Motto, botUser.BotData.Look, botUser.BotData.Gender, botUser.BotData.WalkingEnabled, botUser.BotData.AutomaticChat, botUser.BotData.ChatText, botUser.BotData.SpeakingInterval, botUser.BotData.IsDancing, botUser.BotData.Enable, botUser.BotData.Handitem, botUser.BotData.Status, botUser.BotData.AiType));
        Session.SendPacket(new BotInventoryComposer(Session.User.InventoryComponent.Bots));
        room.RoomUserManager.RemoveBot(botUser.VirtualId, false);
    }
}
