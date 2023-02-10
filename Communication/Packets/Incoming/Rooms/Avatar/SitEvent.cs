namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Games.GameClients;

internal sealed class SitEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var room = session.User.CurrentRoom;        if (room == null)
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);        if (roomUserByUserId == null)
        {
            return;
        }

        if (roomUserByUserId.ContainStatus("sit") || roomUserByUserId.ContainStatus("lay"))
        {
            return;
        }

        if (roomUserByUserId.RotBody % 2 == 0)        {            roomUserByUserId.SetStatus("sit", "0.5");            roomUserByUserId.IsSit = true;            roomUserByUserId.UpdateNeeded = true;        }
    }
}
