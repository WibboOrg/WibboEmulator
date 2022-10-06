namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Inventory.Bots;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Database.Daos.Bot;

internal class PickUpBotEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        var BotId = packet.PopInt();
        if (BotId <= 0)
        {
            return;
        }

        var Room = session.GetUser().CurrentRoom;
        if (Room == null || !Room.CheckRights(session, true))
        {
            return;
        }

        if (!Room.GetRoomUserManager().TryGetBot(BotId, out var BotUser))
        {
            var TargetUser = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(BotId);
            if (TargetUser == null)
            {
                return;
            }

            //Check some values first, please!
            if (TargetUser.GetClient() == null || TargetUser.GetClient().GetUser() == null)
            {
                return;
            }

            TargetUser.TransfBot = false;

            //Quickly remove the old user instance.
            Room.SendPacket(new UserRemoveComposer(TargetUser.VirtualId));

            //Add the new one, they won't even notice a thing!!11 8-)
            Room.SendPacket(new UsersComposer(TargetUser));
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotUserDao.UpdateRoomId(dbClient, BotId);
        }

        _ = session.GetUser().GetInventoryComponent().TryAddBot(new Bot(BotUser.BotData.Id, BotUser.BotData.OwnerId, BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender, BotUser.BotData.WalkingEnabled, BotUser.BotData.AutomaticChat, BotUser.BotData.ChatText, BotUser.BotData.SpeakingInterval, BotUser.BotData.IsDancing, BotUser.BotData.Enable, BotUser.BotData.Handitem, BotUser.BotData.Status));
        session.SendPacket(new BotInventoryComposer(session.GetUser().GetInventoryComponent().GetBots()));
        Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
    }
}
