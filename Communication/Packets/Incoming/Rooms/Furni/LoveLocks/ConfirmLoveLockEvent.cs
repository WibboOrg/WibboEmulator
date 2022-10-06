namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.LoveLocks;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class ConfirmLoveLockEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var Id = packet.PopInt();
        var isConfirmed = packet.PopBoolean();

        var Room = session.GetUser().CurrentRoom;
        if (Room == null || !Room.CheckRights(session))
        {
            return;
        }

        var Item = Room.GetRoomItemHandler().GetItem(Id);

        if (Item == null || Item.GetBaseItem() == null || Item.GetBaseItem().InteractionType != InteractionType.LOVELOCK)
        {
            return;
        }

        var UserOneId = Item.InteractingUser;
        var UserTwoId = Item.InteractingUser2;

        var UserOne = Room.GetRoomUserManager().GetRoomUserByUserId(UserOneId);
        var UserTwo = Room.GetRoomUserManager().GetRoomUserByUserId(UserTwoId);

        if (UserOne == null || UserTwo == null)
        {
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", session.Langue));
            return;
        }
        else if (UserOne.GetClient() == null || UserTwo.GetClient() == null)
        {
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", session.Langue));
            return;
        }
        else if (UserOne == null)
        {
            UserTwo.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", UserTwo.GetClient().Langue));
            UserTwo.LLPartner = 0;
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
            return;
        }
        else if (UserTwo == null)
        {
            UserOne.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", UserOne.GetClient().Langue));
            UserOne.LLPartner = 0;
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
            return;
        }
        else if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
        {
            UserTwo.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", UserTwo.GetClient().Langue));
            UserTwo.LLPartner = 0;

            UserOne.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", UserOne.GetClient().Langue));
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
            if (UserOneId == session.GetUser().Id)
            {
                session.SendPacket(new LoveLockDialogueSetLockedComposer(Id));
                UserOne.LLPartner = UserTwoId;
            }
            else if (UserTwoId == session.GetUser().Id)
            {
                session.SendPacket(new LoveLockDialogueSetLockedComposer(Id));
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

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                ItemDao.UpdateExtradata(dbClient, Item.Id, Item.ExtraData);
            }
        }
    }
}
