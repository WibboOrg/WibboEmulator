namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Games.GameClients;

internal sealed class SitEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        if (roomUserByUserId.ContainStatus("sit") || roomUserByUserId.ContainStatus("lay") || roomUserByUserId.RidingHorse)
        {
            return;
        }

        if (roomUserByUserId.RotBody % 2 == 0)
        {
            roomUserByUserId.SetStatus("sit", "0.5");
            roomUserByUserId.IsSit = true;
            roomUserByUserId.UpdateNeeded = true;
        }
    }
}
