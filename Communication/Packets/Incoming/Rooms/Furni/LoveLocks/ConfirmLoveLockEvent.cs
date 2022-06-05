using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ConfirmLoveLockEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int Id = Packet.PopInt();
            bool isConfirmed = Packet.PopBoolean();

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            Item Item = Room.GetRoomItemHandler().GetItem(Id);

            if (Item == null || Item.GetBaseItem() == null || Item.GetBaseItem().InteractionType != InteractionType.LOVELOCK)
            {
                return;
            }

            int UserOneId = Item.InteractingUser;
            int UserTwoId = Item.InteractingUser2;

            RoomUser UserOne = Room.GetRoomUserManager().GetRoomUserByUserId(UserOneId);
            RoomUser UserTwo = Room.GetRoomUserManager().GetRoomUserByUserId(UserTwoId);

            if (UserOne == null && UserTwo == null)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", Session.Langue));
                return;
            }
            else if (UserOne.GetClient() == null || UserTwo.GetClient() == null)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", Session.Langue));
                return;
            }
            else if (UserOne == null)
            {
                UserTwo.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", UserTwo.GetClient().Langue));
                UserTwo.LLPartner = 0;
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if (UserTwo == null)
            {
                UserOne.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", UserOne.GetClient().Langue));
                UserOne.LLPartner = 0;
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                UserTwo.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", UserTwo.GetClient().Langue));
                UserTwo.LLPartner = 0;

                UserOne.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", UserOne.GetClient().Langue));
                UserOne.LLPartner = 0;

                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if (!isConfirmed)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;

                UserOne.LLPartner = 0;
                UserTwo.LLPartner = 0;
                return;
            }
            else
            {
                if (UserOneId == Session.GetUser().Id)
                {
                    Session.SendPacket(new LoveLockDialogueSetLockedComposer(Id));
                    UserOne.LLPartner = UserTwoId;
                }
                else if (UserTwoId == Session.GetUser().Id)
                {
                    Session.SendPacket(new LoveLockDialogueSetLockedComposer(Id));
                    UserTwo.LLPartner = UserOneId;
                }

                if (UserOne.LLPartner == 0 || UserTwo.LLPartner == 0)
                {
                    return;
                }
                else
                {
                    Item.ExtraData = "1" + (char)5 + UserOne.GetUsername() + (char)5 + UserTwo.GetUsername() + (char)5 + UserOne.GetClient().GetUser().Look + (char)5 + UserTwo.GetClient().GetUser().Look + (char)5 + DateTime.Now.ToString("dd/MM/yyyy");

                    Item.InteractingUser = 0;
                    Item.InteractingUser2 = 0;

                    UserOne.LLPartner = 0;
                    UserTwo.LLPartner = 0;

                    Item.UpdateState(true, true);

                    UserOne.GetClient().SendPacket(new LoveLockDialogueCloseComposer(Id));
                    UserTwo.GetClient().SendPacket(new LoveLockDialogueCloseComposer(Id));

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateExtradata(dbClient, Item.Id, Item.ExtraData);
                    }
                }
            }
        }
    }
}
