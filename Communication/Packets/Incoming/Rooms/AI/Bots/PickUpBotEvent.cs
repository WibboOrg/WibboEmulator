using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users.Inventory.Bots;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PickUpBotEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            int BotId = Packet.PopInt();
            if (BotId <= 0)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            if (!Room.GetRoomUserManager().TryGetBot(BotId, out RoomUser BotUser))
            {
                RoomUser TargetUser = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(BotId);
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

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotUserDao.UpdateRoomId(dbClient, BotId);
            }

            Session.GetUser().GetInventoryComponent().TryAddBot(new Bot(BotUser.BotData.Id, BotUser.BotData.OwnerId, BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender, BotUser.BotData.WalkingEnabled, BotUser.BotData.AutomaticChat, BotUser.BotData.ChatText, BotUser.BotData.SpeakingInterval, BotUser.BotData.IsDancing, BotUser.BotData.Enable, BotUser.BotData.Handitem, BotUser.BotData.Status));
            Session.SendPacket(new BotInventoryComposer(Session.GetUser().GetInventoryComponent().GetBots()));
            Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
        }
    }
}
