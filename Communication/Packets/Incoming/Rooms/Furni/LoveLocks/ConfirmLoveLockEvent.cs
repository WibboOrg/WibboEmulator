namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.LoveLocks;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class ConfirmLoveLockEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var id = packet.PopInt();
        var isConfirmed = packet.PopBoolean();

        var room = session.GetUser().CurrentRoom;
        if (room == null || !room.CheckRights(session))
        {
            return;
        }

        var item = room.RoomItemHandling.GetItem(id);

        if (item == null || item.GetBaseItem() == null || item.GetBaseItem().InteractionType != InteractionType.LOVELOCK)
        {
            return;
        }

        var userOneId = item.InteractingUser;
        var userTwoId = item.InteractingUser2;

        var userOne = room.RoomUserManager.GetRoomUserByUserId(userOneId);
        var userTwo = room.RoomUserManager.GetRoomUserByUserId(userTwoId);

        if (userOne == null || userTwo == null)
        {
            item.InteractingUser = 0;
            item.InteractingUser2 = 0;
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", session.Langue));
            return;
        }
        else if (userOne.Client == null || userTwo.Client == null)
        {
            item.InteractingUser = 0;
            item.InteractingUser2 = 0;
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", session.Langue));
            return;
        }
        else if (userOne == null)
        {
            userTwo.Client.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", userTwo.Client.Langue));
            userTwo.LLPartner = 0;
            item.InteractingUser = 0;
            item.InteractingUser2 = 0;
            return;
        }
        else if (userTwo == null)
        {
            userOne.Client.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", userOne.Client.Langue));
            userOne.LLPartner = 0;
            item.InteractingUser = 0;
            item.InteractingUser2 = 0;
            return;
        }
        else if (item.ExtraData.Contains(Convert.ToChar(5).ToString()))
        {
            userTwo.Client.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", userTwo.Client.Langue));
            userTwo.LLPartner = 0;

            userOne.Client.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", userOne.Client.Langue));
            userOne.LLPartner = 0;

            item.InteractingUser = 0;
            item.InteractingUser2 = 0;
            return;
        }
        else if (!isConfirmed)
        {
            item.InteractingUser = 0;
            item.InteractingUser2 = 0;

            userOne.LLPartner = 0;
            userTwo.LLPartner = 0;
            return;
        }
        else
        {
            if (userOneId == session.GetUser().Id)
            {
                session.SendPacket(new LoveLockDialogueSetLockedComposer(id));
                userOne.LLPartner = userTwoId;
            }
            else if (userTwoId == session.GetUser().Id)
            {
                session.SendPacket(new LoveLockDialogueSetLockedComposer(id));
                userTwo.LLPartner = userOneId;
            }

            if (userOne.LLPartner == 0 || userTwo.LLPartner == 0)
            {
                return;
            }
            else
            {
                item.ExtraData = "1" + (char)5 + userOne.GetUsername() + (char)5 + userTwo.GetUsername() + (char)5 + userOne.Client.GetUser().Look + (char)5 + userTwo.Client.GetUser().Look + (char)5 + DateTime.Now.ToString("dd/MM/yyyy");

                item.InteractingUser = 0;
                item.InteractingUser2 = 0;

                userOne.LLPartner = 0;
                userTwo.LLPartner = 0;

                item.UpdateState(true, true);

                userOne.Client.SendPacket(new LoveLockDialogueCloseComposer(id));
                userTwo.Client.SendPacket(new LoveLockDialogueCloseComposer(id));

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                ItemDao.UpdateExtradata(dbClient, item.Id, item.ExtraData);
            }
        }
    }
}
