namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

internal sealed class RpBotChooseEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var message = packet.PopString();

        if (session == null || session.User == null)
        {
            return;
        }

        var room = session.User.Room;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null)
        {
            return;
        }

        if (message == ("play_slot_" + user.SlotAmount) && user.IsSlot && !user.IsSlotSpin && session.User.WibboPoints >= user.SlotAmount)
        {
            using var dbClient = DatabaseManager.Connection;

            var ownerUser = UserManager.GetUserById(room.RoomData.OwnerId);

            if (ownerUser == null || ownerUser.WibboPoints < user.SlotAmount)
            {
                return;
            }

            user.IsSlotSpin = true;
            user.IsSlot = false;
            user.IsSlotWinner = false;

            var isWin = WibboEnvironment.GetRandomNumber(0, 10000) <= 4800;
            if (isWin)
            {
                user.IsSlotWinner = true;
                session.User.WibboPoints += user.SlotAmount;
                UserDao.UpdateAddPoints(dbClient, session.User.Id, user.SlotAmount);

                ownerUser.WibboPoints -= user.SlotAmount;
                UserDao.UpdateRemovePoints(dbClient, room.RoomData.OwnerId, user.SlotAmount);
            }
            else
            {
                session.User.WibboPoints -= user.SlotAmount;
                UserDao.UpdateRemovePoints(dbClient, session.User.Id, user.SlotAmount);

                ownerUser.WibboPoints += user.SlotAmount;
                UserDao.UpdateAddPoints(dbClient, room.RoomData.OwnerId, user.SlotAmount);
            }

            ownerUser.Client?.SendPacket(new ActivityPointNotificationComposer(ownerUser.WibboPoints, 0, 105));

            LogSlotMachineDao.Insert(dbClient, session.User.Id, user.SlotAmount, isWin);
        }

        _ = room.AllowsShous(user, message);
    }
}
