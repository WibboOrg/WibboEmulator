namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;

using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

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

        var room = session.User.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null)
        {
            return;
        }

        if (message == "play_slot" && user.IsSlot && !user.IsSlotSpin && session.User.WibboPoints >= user.SlotAmount)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            user.IsSlotSpin = true;
            user.IsSlot = false;
            user.IsSlotWinner = false;

            var isWin = WibboEnvironment.GetRandomNumber(0, 100) <= 48;
            if (isWin)
            {
                user.IsSlotWinner = true;
                session.User.WibboPoints += user.SlotAmount;
                UserDao.UpdateAddPoints(dbClient, session.User.Id, user.SlotAmount);
            }
            else
            {
                session.User.WibboPoints -= user.SlotAmount;
                UserDao.UpdateRemovePoints(dbClient, session.User.Id, user.SlotAmount);
            }

            LogSlotMachineDao.Insert(dbClient, session.User.Id, user.SlotAmount, isWin);
        }

        _ = room.AllowsShous(user, message);
    }
}
