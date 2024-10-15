namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users;

internal sealed class RpBotChooseEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var message = packet.PopString();

        if (Session == null || Session.User == null)
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (user == null)
        {
            return;
        }

        if (message == "play_slot" && user.IsSlot && !user.IsSlotSpin && Session.User.WibboPoints >= user.SlotAmount)
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

            var isWin = WibboEnvironment.GetRandomNumber(0, 100) <= 48;
            if (isWin)
            {
                user.IsSlotWinner = true;
                Session.User.WibboPoints += user.SlotAmount;
                UserDao.UpdateAddPoints(dbClient, Session.User.Id, user.SlotAmount);

                ownerUser.WibboPoints -= user.SlotAmount;
                UserDao.UpdateRemovePoints(dbClient, room.RoomData.OwnerId, user.SlotAmount);
            }
            else
            {
                Session.User.WibboPoints -= user.SlotAmount;
                UserDao.UpdateRemovePoints(dbClient, Session.User.Id, user.SlotAmount);

                ownerUser.WibboPoints += user.SlotAmount;
                UserDao.UpdateAddPoints(dbClient, room.RoomData.OwnerId, user.SlotAmount);
            }

            ownerUser.Client?.SendPacket(new ActivityPointNotificationComposer(ownerUser.WibboPoints, 0, 105));

            LogSlotMachineDao.Insert(dbClient, Session.User.Id, user.SlotAmount, isWin);
        }

        _ = room.AllowsShous(user, message);
    }
}
